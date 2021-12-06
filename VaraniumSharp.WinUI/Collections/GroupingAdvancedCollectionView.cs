﻿/*
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

namespace VaraniumSharp.WinUI.Collections
{
    /// <summary>
    /// A collection view implementation that supports filtering, sorting and grouping
    /// </summary>
    public class GroupingAdvancedCollectionView : ExtendedAdvancedCollectionView
    {
        #region Constructor

        /// <summary>
        /// Construct and set the source
        /// </summary>
        /// <param name="source">Source collection for the grouping collection</param>
        public GroupingAdvancedCollectionView(IList source)
            : base(source, true)
        { }

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
                    CollectionGroups = new ObservableVector<object>();
                    RebuildGroups();
                    VectorChanged += HandleViewChanges;
                }
                else
                {
                    //CollectionGroups?.Clear();
                    CollectionGroups = null;
                    VectorChanged -= HandleViewChanges;
                }
            }
        }

        #endregion

        #region Private Methods

        private void AddGroupedItem(object key, object item)
        {
            if (CollectionGroups == null)
            {
                throw new InvalidOperationException("Cannot group items as the internal collection group is null");
            }

            var col = CollectionGroups
                .Cast<CollectionViewGroup>()
                .FirstOrDefault(g => Comparer.Default.Compare(g.Group, key) == 0);
            
            if (col == null)
            {
                col = new CollectionViewGroup(key);
                var keys = CollectionGroups
                    .Cast<CollectionViewGroup>()
                    .Select(x => x.Group)
                    .ToList();

                keys.Add(key);
                keys.Sort();
                var insertIndex = keys.IndexOf(key);

                CollectionGroups.Insert(insertIndex, col);
            }

            col.Items.IsVectorChangedDeferred = ((ObservableVector<object>)CollectionGroups).IsVectorChangedDeferred;
            col.GroupItems.Add(item);
        }

        private object? GetItemGroup(object item)
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
                    var entry = GetItemGroup(this[ndx]);
                    if (entry != null)
                    {
                        AddGroupedItem(entry, this[ndx]);
                    }
                    break;
                case CollectionChange.ItemInserted:
                    var insertEntry = GetItemGroup(this[ndx]);
                    if (insertEntry != null)
                    {
                        AddGroupedItem(insertEntry, this[ndx]);
                    }
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
            if (CollectionGroups == null)
            {
                throw new InvalidOperationException("Cannot group items as the internal collection group is null");
            }

            ((ObservableVector<object>)CollectionGroups).IsVectorChangedDeferred = true;
            try
            {
                CollectionGroups.Clear();

                if (Source == null)
                {
                    return;
                }

                var groupKeys = new Dictionary<object, List<object>>();
                foreach (var item in _view)
                {
                    var key = GetItemGroup(item);
                    if (key == null)
                    {
                        throw new InvalidOperationException("Cannot group items if the key is null");
                    }

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
            finally
            {
                foreach (CollectionViewGroup col in CollectionGroups)
                {
                    col.Items.IsVectorChangedDeferred = false;
                }
                ((ObservableVector<object>)CollectionGroups).IsVectorChangedDeferred = false;
            }
        }

        private void RemoveGroupedItem(object item)
        {
            if (CollectionGroups == null)
            {
                throw new InvalidOperationException("Cannot group items as the internal collection group is null");
            }

            foreach (var g in CollectionGroups.Select(x => ((CollectionViewGroup)x).Items))
            {
                g.IsVectorChangedDeferred = ((ObservableVector<object>)CollectionGroups).IsVectorChangedDeferred;
                g.Remove(item);
            }
        }

        #endregion

        #region Variables

        private Func<object, object>? _group;

        #endregion
    }
}