// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Original code is here: https://github.com/CommunityToolkit/WindowsCommunityToolkit/tree/63cbb4a51bdef59083ccfc86bbcba41f966d0027/Microsoft.Toolkit.Uwp.UI/AdvancedCollectionView
// In general the code is the same as the original except for:
// - Code cleanup using the NineTail Labs styling rules
// - Added nullability (?) where required
// - Added some null checks where required
// - Changed the Compare method to support nested property sorting

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using CommunityToolkit.WinUI.Helpers;
using CommunityToolkit.WinUI.UI;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Data;
using VaraniumSharp.Logging;

namespace VaraniumSharp.WinUI.Collections
{
    /// <summary>
    /// A collection view implementation that supports filtering, sorting and incremental loading
    /// </summary>
    public partial class ExtendedAdvancedCollectionView : IAdvancedCollectionView, INotifyPropertyChanged, ISupportIncrementalLoading, IComparer<object>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedAdvancedCollectionView"/> class.
        /// </summary>
        public ExtendedAdvancedCollectionView()
            : this(new List<object>(0))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedAdvancedCollectionView"/> class.
        /// </summary>
        /// <param name="source">source IEnumerable</param>
        /// <param name="isLiveShaping">Denotes whether this ACV should re-filter/re-sort if a PropertyChanged is raised for an observed property.</param>
        public ExtendedAdvancedCollectionView(IList source, bool isLiveShaping = false)
        {
            _liveShapingEnabled = isLiveShaping;
            _view = [];
            _sortDescriptions = [];
            _sortDescriptions.CollectionChanged += SortDescriptions_CollectionChanged;
            _sortProperties = new();
            Source = source;
            _logger = StaticLogger.GetLogger<ExtendedAdvancedCollectionView>();
        }

        #endregion

        #region Events

        /// <summary>
        /// Current item changed event handler
        /// </summary>
        public event EventHandler<object?>? CurrentChanged;

        /// <summary>
        /// Current item changing event handler
        /// </summary>
        public event CurrentChangingEventHandler? CurrentChanging;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Fired when the sort order of the items in the collection changes
        /// </summary>
        protected event EventHandler? SortChanged;

        /// <summary>
        /// Occurs when the vector changes.
        /// </summary>
        public event VectorChangedEventHandler<object>? VectorChanged;

        /// <summary>
        /// Fired when the view has changed and the <see cref="CollectionGroups"/> isn't null
        /// </summary>
        protected event VectorChangedEventHandler<object>? ViewChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this CollectionView can filter its items
        /// </summary>
        public bool CanFilter => true;

        /// <summary>
        /// Gets a value indicating whether this CollectionView can sort its items
        /// </summary>
        public bool CanSort => true;

        /// <summary>
        /// Gets the groups in collection
        /// </summary>
        public IObservableVector<object>? CollectionGroups { get; protected set; }

        /// <inheritdoc />
        public int Count => _view.Count;

        /// <summary>
        /// Gets or sets the current item
        /// </summary>
        public virtual object? CurrentItem
        {
            get => CurrentPosition > -1 && CurrentPosition < _view.Count ? _view[CurrentPosition] : null;
            set => MoveCurrentTo(value);
        }

        /// <summary>
        /// Gets the position of current item
        /// </summary>
        public virtual int CurrentPosition { get; private set; }

        /// <summary>
        /// Gets or sets the predicate used to filter the visible items
        /// </summary>
        public Predicate<object>? Filter
        {
            get => _filter;

            set
            {
                if (_filter == value)
                {
                    return;
                }

                _filter = value;
                HandleFilterChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the source has more items
        /// </summary>
        public bool HasMoreItems => (_source as ISupportIncrementalLoading)?.HasMoreItems ?? false;

        /// <summary>
        /// Gets a value indicating whether the current item is after the last visible item
        /// </summary>
        public bool IsCurrentAfterLast => CurrentPosition >= _view.Count;

        /// <summary>
        /// Gets a value indicating whether the current item is before the first visible item
        /// </summary>
        public bool IsCurrentBeforeFirst => CurrentPosition < 0;

        /// <inheritdoc />
        public bool IsReadOnly => _source == null || _source.IsReadOnly;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public virtual object this[int index]
        {
            get => _view[index];
            set => _view[index] = value;
        }

        /// <summary>
        /// Gets SortDescriptions to sort the visible items
        /// </summary>
        public IList<SortDescription> SortDescriptions => _sortDescriptions;

        /// <summary>
        /// Gets or sets the source
        /// </summary>
        public IList? Source
        {
            get => _source;

            set
            {
                // ReSharper disable once PossibleUnintendedReferenceComparison
                if (_source == value)
                {
                    return;
                }

                if (_source != null)
                {
                    DetachPropertyChangedHandler(_source);
                }

                _source = value;
                AttachPropertyChangedHandler(_source);

                _sourceWeakEventListener?.Detach();

                if (_source is INotifyCollectionChanged sourceNcc)
                {
                    _sourceWeakEventListener = new(this)
                        {
                            // Call the actual collection changed event
                            OnEventAction = (source, changed, arg3) => SourceNcc_CollectionChanged(source, arg3),

                            // The source doesn't exist anymore
                            OnDetachAction = (listener) => sourceNcc.CollectionChanged -= _sourceWeakEventListener.OnEvent
                        };
                    sourceNcc.CollectionChanged += _sourceWeakEventListener.OnEvent;
                }

                HandleSourceChanged();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the source collection
        /// </summary>
        public IEnumerable? SourceCollection => _source;

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public void Add(object? item)
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            _source?.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            DetachPropertyChangedHandler(_source);

            _source?.Clear();
        }

        /// <inheritdoc/>
        public void ClearObservedFilterProperties()
        {
            _observedFilterProperties.Clear();
        }

        /// <inheritdoc />
        public bool Contains(object item) => _view.Contains(item);

        /// <inheritdoc />
        public void CopyTo(object[] array, int arrayIndex) => _view.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public IEnumerator<object> GetEnumerator() => _view.GetEnumerator();

        /// <inheritdoc />
        public virtual int IndexOf(object? item) => _view.IndexOf(item);

        /// <inheritdoc />
        public void Insert(int index, object? item)
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            _source?.Insert(index, item);
        }

        /// <summary>
        /// Load more items from the source
        /// </summary>
        /// <param name="count">number of items to load</param>
        /// <returns>Async operation of LoadMoreItemsResult</returns>
        /// <exception cref="NotImplementedException">Not implemented yet...</exception>
        public IAsyncOperation<LoadMoreItemsResult>? LoadMoreItemsAsync(uint count)
        {
            var sil = _source as ISupportIncrementalLoading;
            return sil?.LoadMoreItemsAsync(count);
        }

        /// <summary>
        /// Move current index to item
        /// </summary>
        /// <param name="item">item</param>
        /// <returns>success of operation</returns>
        public bool MoveCurrentTo(object? item) => item == CurrentItem || MoveCurrentToIndex(item == null ? -1 : IndexOf(item));

        /// <summary>
        /// Move current item to first item
        /// </summary>
        /// <returns>success of operation</returns>
        public bool MoveCurrentToFirst() => MoveCurrentToIndex(0);

        /// <summary>
        /// Move current item to last item
        /// </summary>
        /// <returns>success of operation</returns>
        public bool MoveCurrentToLast() => MoveCurrentToIndex(_view.Count - 1);

        /// <summary>
        /// Move current item to next item
        /// </summary>
        /// <returns>success of operation</returns>
        public bool MoveCurrentToNext() => MoveCurrentToIndex(CurrentPosition + 1);

        /// <summary>
        /// Moves selected item to position
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>success of operation</returns>
        public bool MoveCurrentToPosition(int index) => MoveCurrentToIndex(index);

        /// <summary>
        /// Move current item to previous item
        /// </summary>
        /// <returns>success of operation</returns>
        public bool MoveCurrentToPrevious() => MoveCurrentToIndex(CurrentPosition - 1);

        /// <inheritdoc/>
        public void ObserveFilterProperty(string propertyName)
        {
            _observedFilterProperties.Add(propertyName);
        }

        /// <summary>
        /// Manually refresh the view
        /// </summary>
        public void Refresh()
        {
            HandleSourceChanged();
        }

        /// <inheritdoc/>
        public void RefreshFilter()
        {
            HandleFilterChanged();
        }

        /// <inheritdoc/>
        public void RefreshSorting()
        {
            HandleSortChanged();
        }

        /// <inheritdoc />
        public bool Remove(object item)
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            _source?.Remove(item);
            return true;
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public void RemoveAt(int index) => Remove(_view[index]);

        #endregion

        #region Private Methods

        private void AttachPropertyChangedHandler(IEnumerable? items)
        {
            if (!_liveShapingEnabled || items == null)
            {
                return;
            }

            foreach (var item in items.OfType<INotifyPropertyChanged>())
            {
                item.PropertyChanged += ItemOnPropertyChanged;
            }
        }

        private void DetachPropertyChangedHandler(IEnumerable? items)
        {
            if (!_liveShapingEnabled || items == null)
            {
                return;
            }

            foreach (var item in items.OfType<INotifyPropertyChanged>())
            {
                item.PropertyChanged -= ItemOnPropertyChanged;
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => _view.GetEnumerator();

        private void HandleFilterChanged()
        {
            if (_filter != null)
            {
                for (var index = 0; index < _view.Count; index++)
                {
                    var item = _view.ElementAt(index);
                    if (_filter(item))
                    {
                        continue;
                    }

                    RemoveFromView(index, item);
                    index--;
                }
            }

            var viewHash = new HashSet<object>(_view);
            var viewIndex = 0;
            for (var index = 0; index < _source?.Count; index++)
            {
                var item = _source[index];
                if (item == null || viewHash.Contains(item))
                {
                    viewIndex++;
                    continue;
                }

                if (HandleItemAdded(index, item, viewIndex))
                {
                    viewIndex++;
                }
            }
        }

        private bool HandleItemAdded(int newStartingIndex, object newItem, int? viewIndex = null)
        {
            if (_filter != null && !_filter(newItem))
            {
                return false;
            }

            var newViewIndex = _view.Count;

            if (_sortDescriptions.Any())
            {
                newViewIndex = _view.BinarySearch(newItem, this);
                if (newViewIndex < 0)
                {
                    newViewIndex = ~newViewIndex;
                }
            }
            else if (_filter != null)
            {
                if (_source == null)
                {
                    HandleSourceChanged();
                    return false;
                }

                if (newStartingIndex == 0 || _view.Count == 0)
                {
                    newViewIndex = 0;
                }
                else if (newStartingIndex == _source.Count - 1)
                {
                    newViewIndex = _view.Count - 1;
                }
                else if (viewIndex.HasValue)
                {
                    newViewIndex = viewIndex.Value;
                }
                else
                {
                    for (int i = 0, j = 0; i < _source.Count; i++)
                    {
                        if (i == newStartingIndex)
                        {
                            newViewIndex = j;
                            break;
                        }

                        if (_view[j] == _source[i])
                        {
                            j++;
                        }
                    }
                }
            }

            _view.Insert(newViewIndex, newItem);
            if (newViewIndex <= CurrentPosition)
            {
                CurrentPosition++;
            }

            var e = new VectorChangedEventArgs(CollectionChange.ItemInserted, newViewIndex, newItem);

            if (CollectionGroups == null)
            {
                OnVectorChanged(e);
            }
            else
            {
                ViewChanged?.Invoke(this, e);
            }
            
            return true;
        }

        private void HandleItemRemoved(int oldStartingIndex, object oldItem)
        {
            if (_filter != null && !_filter(oldItem))
            {
                return;
            }

            if (oldStartingIndex < 0 || oldStartingIndex >= _view.Count || !Equals(_view[oldStartingIndex], oldItem))
            {
                oldStartingIndex = _view.IndexOf(oldItem);
            }

            if (oldStartingIndex < 0)
            {
                return;
            }

            RemoveFromView(oldStartingIndex, oldItem);
        }

        private void HandleSortChanged()
        {
            _view.Sort(this);
            if (CollectionGroups == null)
            {
                OnVectorChanged(new VectorChangedEventArgs(CollectionChange.Reset));
            }
            else
            {
                SortChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void HandleSourceChanged()
        {
            var currentItem = CurrentItem;
            _view.Clear();
            foreach (var item in Source ?? new List<object>())
            {
                if (_filter != null && !_filter(item))
                {
                    continue;
                }

                if (_sortDescriptions.Any())
                {
                    var targetIndex = _view.BinarySearch(item, this);
                    if (targetIndex < 0)
                    {
                        targetIndex = ~targetIndex;
                    }

                    _view.Insert(targetIndex, item);
                }
                else
                {
                    _view.Add(item);
                }
            }

            if (CollectionGroups == null)
            {
                OnVectorChanged(new VectorChangedEventArgs(CollectionChange.Reset));
            }
            else
            {
                ViewChanged?.Invoke(this, new VectorChangedEventArgs(CollectionChange.Reset));
            }


            MoveCurrentTo(currentItem);
        }

        /// <summary>
        /// Occurs when the property of an item in the collection changes
        /// </summary>
        /// <param name="item">Item on which the property changed</param>
        /// <param name="e">Property changed arguments</param>
        protected virtual void ItemOnPropertyChanged(object? item, PropertyChangedEventArgs e)
        {
            if (!_liveShapingEnabled || item == null || _source == null)
            {
                return;
            }

            var filterResult = _filter?.Invoke(item);

            if (filterResult.HasValue && _observedFilterProperties.Contains(e.PropertyName ?? string.Empty))
            {
                var viewIndex = _view.IndexOf(item);
                if (viewIndex != -1 && !filterResult.Value)
                {
                    RemoveFromView(viewIndex, item);
                }
                else if (viewIndex == -1 && filterResult.Value)
                {
                    var index = _source.IndexOf(item);
                    HandleItemAdded(index, item);
                }
            }

            if ((filterResult ?? true) && SortDescriptions.Any(sd => sd.PropertyName == e.PropertyName))
            {
                var oldIndex = _view.IndexOf(item);

                // Check if item is in view:
                if (oldIndex < 0)
                {
                    return;
                }

                _view.RemoveAt(oldIndex);
                var targetIndex = _view.BinarySearch(item, this);
                if (targetIndex < 0)
                {
                    targetIndex = ~targetIndex;
                }

                // Only trigger expensive UI updates if the index really changed:
                if (targetIndex != oldIndex)
                {
                    OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemRemoved, oldIndex, item));

                    _view.Insert(targetIndex, item);

                    OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemInserted, targetIndex, item));
                }
                else
                {
                    _view.Insert(targetIndex, item);
                }
            }
            else if (string.IsNullOrEmpty(e.PropertyName))
            {
                HandleSourceChanged();
            }
        }

        private bool MoveCurrentToIndex(int i)
        {
            try
            {
                if (i < -1 || i >= _view.Count)
                {
                    return false;
                }

                if (i == CurrentPosition)
                {
                    return false;
                }

                var e = new CurrentChangingEventArgs();
                OnCurrentChanging(e);
                if (e.Cancel)
                {
                    return false;
                }

                CurrentPosition = i;
                OnCurrentChanged(null);
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error moving current item to index");
                return false;
            }
        }

        /// <summary>
        /// Property changed event invoker
        /// </summary>
        /// <param name="propertyName">name of the property that changed</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        private void RemoveFromView(int itemIndex, object item)
        {
            _view.RemoveAt(itemIndex);
            if (itemIndex <= CurrentPosition)
            {
                CurrentPosition--;
            }

            var e = new VectorChangedEventArgs(CollectionChange.ItemRemoved, itemIndex, item);

            if (CollectionGroups == null)
            {
                OnVectorChanged(e);
            }
            else
            {
                ViewChanged?.Invoke(this, e);
            }
        }

        private void SortDescriptions_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_deferCounter > 0)
            {
                return;
            }

            _sortProperties.Clear();
            HandleSortChanged();
        }

        private void SourceNcc_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AttachPropertyChangedHandler(e.NewItems);
                    if (_deferCounter <= 0)
                    {
                        var newItem = e.NewItems?[0];
                        if (e.NewItems?.Count == 1 && newItem != null)
                        {
                            HandleItemAdded(e.NewStartingIndex, newItem);
                        }
                        else
                        {
                            HandleSourceChanged();
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    DetachPropertyChangedHandler(e.OldItems);
                    if (_deferCounter <= 0)
                    {
                        var oldItem = e.OldItems?[0];
                        if (e.OldItems?.Count == 1 && oldItem != null)
                        {
                            HandleItemRemoved(e.OldStartingIndex, oldItem);
                        }
                        else
                        {
                            HandleSourceChanged();
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    if (_deferCounter <= 0)
                    {
                        HandleSourceChanged();
                    }

                    break;
            }
        }

        #endregion

        #region Variables

        private readonly bool _liveShapingEnabled;

        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly ILogger _logger;

        private readonly HashSet<string> _observedFilterProperties = [];

        private readonly ObservableCollection<SortDescription> _sortDescriptions;

        private readonly Dictionary<string, PropertyInfo> _sortProperties;

        /// <summary>
        /// The view storing the collection of items
        /// </summary>
        protected readonly List<object> _view;

        private int _deferCounter;

        private Predicate<object>? _filter;

        private IList? _source;

        private WeakEventListener<ExtendedAdvancedCollectionView, object, NotifyCollectionChangedEventArgs>? _sourceWeakEventListener;

        #endregion
    }
}