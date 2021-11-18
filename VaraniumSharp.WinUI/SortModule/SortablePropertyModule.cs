using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml.Data;
using VaraniumSharp.WinUI.Collections;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Assist with sorting a <see cref="ICollectionView"/> based on <see cref="SortablePropertyAttribute"/>s
    /// </summary>
    public sealed class SortablePropertyModule : ShapingPropertyModuleBase<SortablePropertyAttribute>
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="viewSourceToSort">
        /// Collection view to sort. Note the collection should have been constructed with isLiveShaping set to true.
        /// The <see cref="ExtendedAdvancedCollectionView"/> or <see cref="GroupingAdvancedCollectionView"/> is recommended.
        /// </param>
        public SortablePropertyModule(IAdvancedCollectionView viewSourceToSort)
            : base(viewSourceToSort)
        {
        }

        #endregion

        #region Private Methods

        /// <inheritdoc />
        protected override ShapingEntry? CreateShapingEntry(string propertyName, ShapingPropertyAttributeBase attribute)
        {
            if (attribute is SortablePropertyAttribute sortAttribute)
            {
                return new SortableShapingEntry
                {
                    PropertyName = propertyName,
                    Header = attribute.Header,
                    Tooltip = attribute.ToolTip,
                    SortDirection = sortAttribute.DefaultSortDirection,
                    DefaultDirection = sortAttribute.DefaultSortDirection
                };
            }

            return null;
        }

        /// <inheritdoc />
        protected override void EntriesShapedByOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action is NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Move)
            {
                foreach (var item in e.OldItems ?? new List<object>())
                {
                    if (item is ShapingEntry sortItem)
                    {
                        var descriptionToRemove = ViewSource.SortDescriptions.First(x => x.PropertyName == sortItem.PropertyName);
                        ViewSource.SortDescriptions.Remove(descriptionToRemove);
                        FireShapingChangedEvent();
                    }
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ViewSource.SortDescriptions.Clear();
            }

            if (e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Move)
            {
                foreach (var entry in e.NewItems ?? new List<object>())
                {
                    if (entry is ShapingEntry sortItem)
                    {
                        Shape(sortItem.PropertyName);
                    }
                }
            }

            IsShaped = EntriesShapedBy.Count > 0;
        }

        /// <inheritdoc />
        protected override void Shape(string propertyName)
        {
            if (EntriesShapedBy.First(x => x.PropertyName == propertyName) is not SortableShapingEntry entryToSortBy)
            {
                throw new InvalidOperationException($"{typeof(SortablePropertyModule)} cannot deal with the provided attribute as it is not a SortableShapingEntry");
            }

            var sortProperty = ViewSource.SortDescriptions.FirstOrDefault(x => x.PropertyName == propertyName);
            if (sortProperty != default)
            {
                var index = ViewSource.SortDescriptions.IndexOf(sortProperty);
                ViewSource.SortDescriptions.RemoveAt(index);
                ViewSource.SortDescriptions.Insert(index,
                    new SortDescription(propertyName, entryToSortBy.SortDirection));
            }
            else
            {
                var insertIndex = EntriesShapedBy.IndexOf(entryToSortBy);
                if (insertIndex < ViewSource.SortDescriptions.Count)
                {
                    ViewSource.SortDescriptions.Insert(insertIndex,
                        new SortDescription(propertyName, entryToSortBy.SortDirection));
                }
                else
                {
                    ViewSource.SortDescriptions.Add(new SortDescription(propertyName,
                        entryToSortBy.SortDirection));
                }
            }

            FireShapingChangedEvent();
        }

        #endregion
    }
}