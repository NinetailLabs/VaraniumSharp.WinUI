using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using CommunityToolkit.WinUI.UI;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Part of the SortablePropertyModule that contains the private methods
    /// </summary>
    public partial class SortablePropertyModule
    {
        #region Private Methods

        /// <summary>
        /// Create a button for a Property
        /// </summary>
        /// <param name="propertyName">Name of the property the button is for</param>
        /// <param name="header">Header for the sort control</param>
        /// <param name="toolTip">Tooltip for the button</param>
        /// <param name="defaultSortDirection">Indicate the default sort direction for the property</param>
        private void CreateSortEntry(string propertyName, string header, string toolTip, SortDirection defaultSortDirection)
        {
            var entry = new SortOrderEntry
            {
                PropertyName = propertyName,
                SortHeader = header,
                SortTooltip = toolTip,
                SortDirection = defaultSortDirection,
                DefaultSortDirection = defaultSortDirection
            };
            AvailableSortEntries.Add(entry);

            entry.RequestSortUpdate += EntryOnRequestSortUpdate;
        }

        /// <summary>
        /// Occurs when the <see cref="EntriesSortedBy"/> collection changed and is used to trigger a sort operation
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void EntriesSortedByOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Move)
            {
                foreach (var item in e.OldItems ?? new List<object>())
                {
                    if (item is SortOrderEntry sortItem)
                    {
                        var descriptionToRemove = _viewSourceToSort.SortDescriptions.First(x => x.PropertyName == sortItem.PropertyName);
                        _viewSourceToSort.SortDescriptions.Remove(descriptionToRemove);
                        SortChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _viewSourceToSort.SortDescriptions.Clear();
            }

            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Move)
            {
                foreach (var entry in e.NewItems ?? new List<object>())
                {
                    if (entry is SortOrderEntry sortItem)
                    {
                        Sort(sortItem.PropertyName);
                    }
                }
            }

            IsSorted = EntriesSortedBy.Count > 0;
        }

        /// <summary>
        /// Occurs when one of the sort entries request resorting of the collection
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        /// <exception cref="NotImplementedException"></exception>
        private void EntryOnRequestSortUpdate(object? sender, EventArgs e)
        {
            if (sender is SortOrderEntry sortOrder && EntriesSortedBy.Contains(sortOrder))
            {
                Sort(sortOrder.PropertyName);
            }
        }

        /// <summary>
        /// Get the properties that are decorated with <see cref="SortablePropertyAttribute"/> for a type
        /// </summary>
        /// <param name="type">Type properties should be retrieved for</param>
        /// <returns>Matching properties</returns>
        private static IEnumerable<PropertyInfo> GetPropertiesForType(Type type)
        {
            return type
                .GetProperties()
                .Where(x => x.GetCustomAttributes(typeof(SortablePropertyAttribute), true).Any());
        }

        /// <summary>
        /// Handle the generation of sort entries for nested attributes
        /// </summary>
        /// <param name="attribute">Attribute for which sort entries should be created</param>
        /// <param name="propertyPrefix">Prefix to get to the attribute</param>
        /// <param name="property">Property info for the attribute</param>
        private void HandleNestedAttributeEntryGeneration(SortablePropertyAttribute attribute, string propertyPrefix, PropertyInfo property)
        {
            var nestedTypes = attribute.UseModuleDictionaryToGetSortTypes
                        ? NestedTypeList[attribute.ModuleDictionaryIndex]
                        : attribute.NestedTypes;

            foreach (var attributeNestedType in nestedTypes)
            {
                var nestedProperties = GetPropertiesForType(attributeNestedType);
                HandleSortEntriesGeneration($"{propertyPrefix}{property.Name}.", nestedProperties);
            }
        }

        /// <summary>
        /// Generate buttons for sorting.
        /// This method includes logic to generate buttons for complex nested types
        /// </summary>
        /// <param name="propertyPrefix">Prefix of property for nested types</param>
        /// <param name="propertiesToCreateButtonsFor">Properties that buttons should be created for</param>
        private void HandleSortEntriesGeneration(string propertyPrefix, IEnumerable<PropertyInfo> propertiesToCreateButtonsFor)
        {
            propertiesToCreateButtonsFor = propertiesToCreateButtonsFor
                .OrderBy(x => ((SortablePropertyAttribute?)x.GetCustomAttribute(typeof(SortablePropertyAttribute)))?.Header);

            foreach (var property in propertiesToCreateButtonsFor)
            {
                var attribute = (SortablePropertyAttribute?)property.GetCustomAttribute(typeof(SortablePropertyAttribute));

                if (attribute == null)
                {
                    continue;
                }

                if (attribute.HasNestedProperties)
                {
                    HandleNestedAttributeEntryGeneration(attribute, propertyPrefix, property);
                }
                else
                {
                    var fullPropertyName = $"{propertyPrefix}{property.Name}";
                    CreateSortEntry(fullPropertyName, attribute.Header, attribute.ToolTip, attribute.DefaultSortDirection);
                    if (attribute.IsDefault && string.IsNullOrEmpty(_defaultSortProperty))
                    {
                        _defaultSortProperty = fullPropertyName;
                    }
                }
            }
        }

        /// <summary>
        /// Remove the sort order entry from the collection
        /// </summary>
        /// <param name="collection">Collection the entry should be removed from</param>
        /// <param name="propertyName">Property name of the entry to remove</param>
        /// <returns>Indicate if the entry could be found in the collection</returns>
        private static bool RemoveSortEntryFromCollection(ObservableCollection<SortOrderEntry> collection, string propertyName)
        {
            var entryToRemove = collection.FirstOrDefault(x => x.PropertyName == propertyName);
            if (entryToRemove == null)
            {
                return false;
            }

            collection.Remove(entryToRemove);
            return true;
        }

        /// <summary>
        /// Sort the collection based on a property
        /// </summary>
        /// <param name="propertyName">Name of the property that sorting should be done on</param>
        private void Sort(string propertyName)
        {
            var entryToSortBy = EntriesSortedBy.First(x => x.PropertyName == propertyName);

            var sortProperty = _viewSourceToSort.SortDescriptions.FirstOrDefault(x => x.PropertyName == propertyName);
            if (sortProperty != default)
            {
                var index = _viewSourceToSort.SortDescriptions.IndexOf(sortProperty);
                _viewSourceToSort.SortDescriptions.RemoveAt(index);
                _viewSourceToSort.SortDescriptions.Insert(index, new SortDescription(propertyName, entryToSortBy.SortDirection));
            }
            else
            {
                var insertIndex = EntriesSortedBy.IndexOf(entryToSortBy);
                if (insertIndex < _viewSourceToSort.SortDescriptions.Count)
                {
                    _viewSourceToSort.SortDescriptions.Insert(insertIndex, new SortDescription(propertyName, entryToSortBy.SortDirection));
                }
                else
                {
                    _viewSourceToSort.SortDescriptions.Add(new SortDescription(propertyName,
                        entryToSortBy.SortDirection));
                }
            }

            SortChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}