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
using System.ComponentModel;
using System.Linq;
using Windows.Foundation.Collections;
using Microsoft.Extensions.Logging;
using VaraniumSharp.Logging;
using VaraniumSharp.WinUI.Interfaces.Collections;

namespace VaraniumSharp.WinUI.Collections
{
    /// <summary>
    /// A collection view implementation that supports filtering, sorting and grouping
    /// </summary>
    public class GroupingAdvancedCollectionView : ExtendedAdvancedCollectionView, IGroupingAdvancedCollectionView
    {
        #region Constructor

        /// <summary>
        /// Construct and set the source
        /// </summary>
        /// <param name="source">Source collection for the grouping collection</param>
        public GroupingAdvancedCollectionView(IList source)
            : base(source, true)
        {
            GroupDescriptions = new List<string>();
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

        /// <inheritdoc />
        public override object? CurrentItem
        {
            get
            {
                if (CollectionGroups == null)
                {
                    return base.CurrentItem;
                }

                if (CurrentPosition < 0)
                {
                    return null;
                }
                
                var details = GetEntryGroupAndLocationByIndex(CurrentPosition);
                if (details == null)
                {
                    return null;
                }

                return CurrentPosition > -1 && CurrentPosition < _view.Count ? details.Value.groupContainingEntry.Items[details.Value.entryIndex] : null;
            }
            set => MoveCurrentTo(value);
        }

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
                    SortChanged += OnSortChanged;
                }
                else
                {
                    CollectionGroups = null;
                    ViewChanged -= HandleViewChanges;
                    SortChanged -= OnSortChanged;
                    OnVectorChanged(new VectorChangedEventArgs(CollectionChange.Reset));
                }
            }
        }

        /// <inheritdoc />
        public IList<string> GroupDescriptions { get; }

        /// <inheritdoc />
        public override object this[int index]
        {
            get
            {
                if (CollectionGroups == null)
                {
                    return base[index];
                }
                
                var details = GetEntryGroupAndLocationByIndex(index);
                return details == null 
                    ? base[index] 
                    : details.Value.groupContainingEntry.Items[details.Value.entryIndex];
            }
            set
            {
                if (CollectionGroups == null)
                {
                    base[index] = value;
                }

                var details = GetEntryGroupAndLocationByIndex(index);
                if (details == null)
                {
                    base[index] = value;
                }
                else
                {
                    details.Value.groupContainingEntry.Items[details.Value.entryIndex] = value;
                }

            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override int IndexOf(object? item)
        {
            if (item == null)
            {
                return -1;
            }

            if (CollectionGroups == null)
            {
                return base.IndexOf(item);
            }

            var group = CollectionGroups
                .Cast<CollectionViewGroup>()
                .FirstOrDefault(x => x.GroupItems.Contains(item));

            if (group == null)
            {
                return -1;
            }

            var startIndex = group.StartIndex;
            var itemIndex = group.GroupItems.IndexOf(item);

            return startIndex + itemIndex;
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
                col = new(key);
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

            UpdateGroupStartIndexes();

            if (!col.Items.IsVectorChangedDeferred)
            {
                OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemInserted, col.StartIndex + col.Items.IndexOf(item)));1
            }
        }

        /// <summary>
        /// Retrieve the group and resolution index for an index value.
        /// </summary>
        /// <param name="index">The index in the entire collection to find the group for</param>
        /// <returns>The group containing the index value and the index of the entry in the group</returns>
        /// <exception cref="InvalidOperationException">Thrown if the CollectionGroups are null</exception>
        private (CollectionViewGroup groupContainingEntry, int entryIndex)? GetEntryGroupAndLocationByIndex(int index)
        {
            if (CollectionGroups == null)
            {
                throw new InvalidOperationException($"{nameof(CollectionGroups)} should not be empty");
            }

            var group = index == 0
                ? CollectionGroups.FirstOrDefault()
                : CollectionGroups
                    .Cast<CollectionViewGroup>()
                    .LastOrDefault(x => x.StartIndex <= index);

            if (group is not CollectionViewGroup typedGroup
                || index < typedGroup.StartIndex
                || index >= typedGroup.StartIndex + typedGroup.Items.Count)
            {
                return null;
            }

            var resolutionIndex = index - typedGroup.StartIndex >= typedGroup.GroupItems.Count
                ? index - typedGroup.StartIndex - 1
                : index - typedGroup.StartIndex;

            return new (typedGroup, resolutionIndex);
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
                    _view.Sort(this);
                    if (typedArgs?.Item != null)
                    {
                        RemoveGroupedItem(typedArgs.Item);
                    }
                    var entry = GetItemGroup(base[ndx]);
                    if (entry != null)
                    {
                        AddGroupedItem(entry, base[ndx]);
                    }
                    break;
                case CollectionChange.ItemInserted:
                    _view.Sort(this);
                    var insertEntry = GetItemGroup(base[ndx]);
                    if (insertEntry != null)
                    {
                        AddGroupedItem(insertEntry, base[ndx]);
                    }
                    break;
                case CollectionChange.ItemRemoved:
                    if (typedArgs?.Item != null)
                    {
                        var cGroup = CollectionGroups
                            ?.Select(x => (CollectionViewGroup)x)
                            .FirstOrDefault(x => x.Items.Contains(typedArgs.Item));

                        var items = cGroup
                                        ?.Items
                                        .Count
                                    ?? 0;
                        if (items > 1)
                        {
                            RemoveGroupedItem(typedArgs.Item);
                        }
                        else
                        {
                            RemoveGroupedItem(typedArgs.Item);
                            if (CollectionGroups != null && cGroup != null)
                            {
                                var idx = CollectionGroups.IndexOf(cGroup);
                                CollectionGroups.RemoveAt(idx);
                                UpdateGroupStartIndexes();
                            }
                        }
                    }
                    _view.Sort(this);
                    break;
                case CollectionChange.Reset:
                    RebuildGroups();
                    break;
            }
        }

        /// <inheritdoc />
        protected override void ItemOnPropertyChanged(object? item, PropertyChangedEventArgs e)
        {
            if (item == null)
            {
                return;
            }

            if (CollectionGroups == null)
            {
                base.ItemOnPropertyChanged(item, e);
                return;
            }

            try
            {

                var filterResult = Filter?.Invoke(item);
                var cGroup = CollectionGroups
                    ?.Select(x => (CollectionViewGroup)x)
                    .FirstOrDefault(x => x.Items.Contains(item));
                var key = GetItemGroup(item);
                if (key == null)
                {
                    return;
                }

                if (Comparer.Default.Compare(cGroup?.Group ?? string.Empty, key) != 0)
                {
                    RemoveGroupedItem(item);
                    AddGroupedItem(key, item);
                    if (CollectionGroups != null && cGroup?.GroupItems.Count == 0)
                    {
                        var idx = CollectionGroups.IndexOf(cGroup);
                        CollectionGroups.RemoveAt(idx);
                        UpdateGroupStartIndexes();
                    }
                }

                if ((filterResult ?? true) && SortDescriptions.Any(sd => sd.PropertyName == e.PropertyName))
                {
                    if (Comparer.Default.Compare(cGroup?.Group ?? string.Empty, key) == 0)
                    {
                        var oldIndex = _view.IndexOf(item);
                        if (oldIndex < 0)
                        {
                            return;
                        }

                        // Move the item to the correct position in the view and update the start index of the groups
                        _view.RemoveAt(oldIndex);
                        var targetIndex = _view.BinarySearch(item, this);
                        if (targetIndex < 0)
                        {
                            targetIndex = ~targetIndex;
                        }

                        _view.Insert(targetIndex, item);
                        UpdateGroupStartIndexes();

                        // If our entry didn't move we can exit early
                        if (targetIndex == oldIndex)
                        {
                            return;
                        }

                        // Ignore our entry similar to the AddGroupItem, otherwise our firstGroupItem will likely be the item itself
                        var firstGroupItem = cGroup?.GroupItems.Except([item]).FirstOrDefault();
                        if (firstGroupItem == null || cGroup == null)
                        {
                            return;
                        }

                        var firstIndex = _view.IndexOf(firstGroupItem);
                        var insertIndex = _view.IndexOf(item);
                        var offSet = insertIndex - firstIndex;
                        if (offSet <= cGroup.GroupItems.Count - 1)
                        {
                            // If the item is already in the correct position we can exit early
                            if (cGroup.GroupItems.IndexOf(item) == (offSet < 0 ? 0 : offSet))
                            {
                                return;
                            }
                        }

                        // TODO - There are some strange edge cases where the item will removed and re-added to the same location at the end of the group, however trying to fix this issue can cause items to become stuck in the wrong location
                        //var groupItems = cGroup.GroupItems.Count;
                        //if (offSet > groupItems && cGroup.GroupItems.IndexOf(item) == groupItems - 1)
                        //{
                        //    return;
                        //}
                        RemoveGroupedItem(item);
                        AddGroupedItem(key, item);
                    }
                }
            }
            catch (Exception exception)
            {
                StaticLogger.GetLogger<GroupingAdvancedCollectionView>().LogError(exception, "Error occurred while handling item property {Property} update", e.PropertyName);
            }
        }

        /// <summary>
        /// Occurs when the sorting of the backing collection changes
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void OnSortChanged(object? sender, EventArgs e)
        {
            if (CollectionGroups == null)
            {
                throw new InvalidOperationException("Cannot group items as the internal collection group is null");
            }

            var viewDictionary = new Dictionary<object, List<object>>();
            foreach (var item in _view)
            {
                var key = GetItemGroup(item);
                if (key == null)
                {
                    continue;
                }
                if (viewDictionary.TryGetValue(key, out var value))
                {
                    value.Add(item);
                }
                else
                {
                    viewDictionary.Add(key, [item]);
                }
            }

            foreach (var group in CollectionGroups.Where(x => ((CollectionViewGroup)x).Items.Count > 1).Select(x => (CollectionViewGroup)x))
            {
                var newOrder = viewDictionary[group.Group];
                group.Items.Clear();
                for (var r = 0; r < newOrder.Count; r++)
                {
                    group.Items.Add(newOrder[r]);
                    OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemChanged, group.StartIndex + r));
                }
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

                    if (!groupKeys.TryGetValue(key, out var value))
                    {
                        groupKeys.Add(key, [item]);
                    }
                    else
                    {
                        value.Add(item);
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
            vector.RemoveAt(idx);

            var startIndex = itemGroup.StartIndex;

            if (!vector.IsVectorChangedDeferred)
            {
                OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemRemoved, startIndex + idx));
            }

            UpdateGroupStartIndexes();
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
                var group = (CollectionViewGroup)CollectionGroups[r];
                if (startIndex == 0 && r > 0)
                {
                    var firstGroup = (CollectionViewGroup)CollectionGroups[0];
                    startIndex += firstGroup.Items.Count;
                }

                group.StartIndex = startIndex;
                startIndex += group.Items.Count;
            }
        }

        #endregion

        #region Variables

        private Func<object, object>? _group;

        #endregion
    }
}