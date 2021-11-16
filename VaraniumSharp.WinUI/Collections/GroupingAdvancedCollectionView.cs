/*
 * Original code is from an issue on the WindowsCommunityToolkit GitHub.
 * The issue is here: https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3089
 * Code link: https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3089#issuecomment-604256939
 *
 * Code has been cleaned up and modified as required.
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml.Data;

namespace VaraniumSharp.WinUI.Collections
{
    /// <summary>
    /// A collection view implementation that supports filtering, sorting and grouping
    /// </summary>
    public class GroupingAdvancedCollectionView : ExtendedAdvancedCollectionView, ICollectionView
    {
        #region Constructor

        /// <summary>
        /// Construct and set the source
        /// </summary>
        /// <param name="source">Source collection for the grouping collection</param>
        public GroupingAdvancedCollectionView(IList source)
            : base(source, true)
        {

        }

        /// <summary>
        /// Construct and set the source as well as the grouping function
        /// </summary>
        /// <param name="source">Source collection for the grouping collection</param>
        /// <param name="group">Function used to group the collection</param>
        public GroupingAdvancedCollectionView(IList source, Func<object, object> group)
            : this(source)
        {
            Group = group;
        }

        #endregion

        #region Properties

        // The CollectionGroups property is of type IObservableVector;, but these objects should implement ICollectionViewGroup.
        IObservableVector<object>? ICollectionView.CollectionGroups => _collectionGroups;

        /// <summary>
        /// Function that is used to group the collection items.
        /// Set to null to remove the grouping.
        /// </summary>
        public Func<object, object>? Group
        {
            get => _group;
            set
            {
                _group = value;
                if (value != null)
                {
                    _collectionGroups = new ObservableVector<object>();
                    RebuildGroups();
                    VectorChanged += HandleViewChanges;
                }
                else
                {
                    _collectionGroups = null;
                    VectorChanged -= HandleViewChanges;
                }
            }
        }

        private Func<object, object>? _group;

        #endregion

        #region Private Methods

        private void AddGroupedItem(object key, object item)
        {
            if (_collectionGroups == null)
            {
                throw new InvalidOperationException("Cannot group items as the internal collection group is null");
            }

            var col = _collectionGroups
                .Cast<CollectionViewGroup>()
                .FirstOrDefault(g => Comparer.Default.Compare(g.Group, key) == 0);
            
            if (col == null)
            {
                col = new CollectionViewGroup(key);
                _collectionGroups.Add(col);
            }
            
            col.Items.IsVectorChangedDeferred = _collectionGroups.IsVectorChangedDeferred;
            col.GroupItems.Add(item);
        }

        private object GetItemGroup(object item)
        {
            return Group?.Invoke(item);
        }

        private void HandleViewChanges(IObservableVector<object> sender, IVectorChangedEventArgs args)
        {
            var ndx = (int)args.Index;
            switch (args.CollectionChange)
            {
                case CollectionChange.ItemChanged:
                    RemoveGroupedItem(this[ndx]);
                    AddGroupedItem(GetItemGroup(this[ndx]), this[ndx]);
                    break;
                case CollectionChange.ItemInserted:
                    AddGroupedItem(GetItemGroup(this[ndx]), this[ndx]);
                    break;
                case CollectionChange.ItemRemoved:
                    RebuildGroups();
                    break;
                case CollectionChange.Reset:
                    RebuildGroups();
                    break;
            }
        }

        private void RebuildGroups()
        {
            if (_collectionGroups == null)
            {
                throw new InvalidOperationException("Cannot group items as the internal collection group is null");
            }

            _collectionGroups.IsVectorChangedDeferred = true;
            try
            {
                _collectionGroups.Clear();

                if (Source != null)
                {
                    var groupKeys = new Dictionary<object, List<object>>();
                    foreach (var item in _view)
                    {
                        var key = GetItemGroup(item);
                        if (!groupKeys.ContainsKey(key))
                        {
                            groupKeys.Add(key, new List<object>{ item });
                        }
                        else
                        {
                            groupKeys[key].Add(item);
                        }
                    }

                    foreach (var item in groupKeys.OrderBy(x => x.Key))
                    {
                        foreach (var subItem in item.Value)
                        {
                            AddGroupedItem(item.Key, subItem);
                        }
                    }
                }
            }
            finally
            {
                foreach (CollectionViewGroup col in _collectionGroups)
                {
                    col.Items.IsVectorChangedDeferred = false;
                }
                _collectionGroups.IsVectorChangedDeferred = false;
            }
        }

        private void RemoveGroupedItem(object item)
        {
            if (_collectionGroups == null)
            {
                throw new InvalidOperationException("Cannot group items as the internal collection group is null");
            }

            foreach (CollectionViewGroup g in _collectionGroups)
            {
                g.Items.IsVectorChangedDeferred = _collectionGroups.IsVectorChangedDeferred;
                g.Items.Remove(item);
            }
        }

        #endregion

        #region Variables

        private ObservableVector<object>? _collectionGroups;

        #endregion
    }
}