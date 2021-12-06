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
                    ViewChanged += HandleViewChanges;
                }
                else
                {
                    CollectionGroups = null;
                    ViewChanged -= HandleViewChanges;
                    OnVectorChanged(new VectorChangedEventArgs(CollectionChange.Reset));
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

                if (!col.Items.IsVectorChangedDeferred)
                {
                    OnVectorChanged(new VectorChangedEventArgs(CollectionChange.Reset));
                }
            }

            UpdateGroupStartIndexes();

            col.Items.IsVectorChangedDeferred = ((ObservableVector<object>)CollectionGroups).IsVectorChangedDeferred;

            var firstGroupItem = col.GroupItems.FirstOrDefault();
            if (firstGroupItem != null)
            {
                var firstIndex = _view.IndexOf(firstGroupItem);
                var insertIndex = _view.IndexOf(item);
                var offSet = insertIndex - firstIndex;
                if (offSet > col.GroupItems.Count)
                {
                    col.GroupItems.Add(item);
                }
                else
                {
                    col.GroupItems.Insert(offSet < 0 ? 0 : offSet, item);
                }
            }
            else
            {
                col.GroupItems.Add(item);
            }

            if (!col.Items.IsVectorChangedDeferred)
            {
                OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemInserted,
                    col.StartIndex + col.Items.IndexOf(item)));
            }
        }

        /// <summary>
        /// Update the CollectionGroups` start indexes
        /// </summary>
        private void UpdateGroupStartIndexes()
        {
            if (CollectionGroups == null)
            {
                return;
            }

            var startIndex = 0;
            for (var r = 0; r < CollectionGroups.Count; r++)
            {
                var group = ((CollectionViewGroup)CollectionGroups[r]);
                group.StartIndex = r == 0
                    ? 0
                    : startIndex;
                startIndex += group.Items.Count;
            }
        }

        private object? GetItemGroup(object item)
        {
            return Group?.Invoke(item);
        }

        private void HandleViewChanges(IObservableVector<object> sender, IVectorChangedEventArgs args)
        {
            var typedArgs = args as VectorChangedEventArgs;
            var ndx = (int)args.Index;
            switch (args.CollectionChange)
            {
                case CollectionChange.ItemChanged:
                    if (typedArgs?.Item != null)
                    {
                        RemoveGroupedItem(typedArgs.Item);
                    }
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
                    var doRebuild = true;
                    if (typedArgs?.Item != null)
                    {
                        var items = CollectionGroups
                            ?.Select(x => (CollectionViewGroup)x)
                            .FirstOrDefault(x => x.Items.Contains(typedArgs.Item))
                            ?.Items
                            .Count
                                ?? 0;
                        if (items > 1)
                        {
                            RemoveGroupedItem(typedArgs.Item);
                            doRebuild = false;
                        }
                    }

                    if (doRebuild)
                    {
                        RebuildGroups();
                    }
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
                OnVectorChanged(new VectorChangedEventArgs(CollectionChange.Reset));
            }
        }

        /// <summary>
        /// Remove an item from a group.
        /// Note that this method cannot deal with the removal of empty groups, this requires a group rebuild.
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <exception cref="InvalidOperationException">Thrown if the CollectionGroups collection is null</exception>
        private void RemoveGroupedItem(object item)
        {
            if (CollectionGroups == null)
            {
                throw new InvalidOperationException("Cannot group items as the internal collection group is null");
            }

            if (CollectionGroups.FirstOrDefault(x => ((CollectionViewGroup)x).Items.Contains(item)) is not CollectionViewGroup itemGroup)
            {
                return;
            }
            
            var vector = itemGroup.Items;

            vector.IsVectorChangedDeferred = ((ObservableVector<object>)CollectionGroups).IsVectorChangedDeferred;
            var idx = vector.IndexOf(item);
            vector.Remove(item);

            var startIndex = itemGroup.StartIndex;

            if (!vector.IsVectorChangedDeferred)
            {
                OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemRemoved, startIndex + idx));
            }

            UpdateGroupStartIndexes();
        }

        #endregion

        #region Variables

        private Func<object, object>? _group;

        #endregion
    }
}