using System;
using System.Linq;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Part of the SortablePropertyModule that contains the public methods
    /// </summary>
    public partial class SortablePropertyModule
    {
        #region Public Methods

        /// <summary>
        /// Occurs when the user clicks the sort clear button.
        /// Will clear all current sorts.
        /// </summary>
        public void ClearSortOnClick()
        {
            var entriesToRemove = EntriesSortedBy.ToList();
            foreach (var sortOrderData in entriesToRemove)
            {
                AvailableSortEntries.Add(sortOrderData);
                EntriesSortedBy.Remove(sortOrderData);
            }
        }

        /// <summary>
        /// Generate the buttons for sorting the collection
        /// </summary>
        /// <param name="collectionTypes">
        /// Type(s) of entries in the collection.
        /// Multiple types are allowed in case the interface implements another interface(s) so that those can also contribute sorting properties
        /// </param>
        public void GenerateSortEntries(params Type[] collectionTypes)
        {
            foreach (var collectionType in collectionTypes)
            {
                var sortableProperties = GetPropertiesForType(collectionType);
                GenerateSortEntries(string.Empty, sortableProperties);
            }

            if (!string.IsNullOrEmpty(_defaultSortProperty)
                && !DisableDefaultSort)
            {
                var availableEntry = AvailableSortEntries.First(x => x.PropertyName == _defaultSortProperty);
                AvailableSortEntries.Remove(availableEntry);
                EntriesSortedBy.Add(availableEntry);
            }
        }

        /// <summary>
        /// Removes a sorting property using the name of the property
        /// </summary>
        /// <param name="propertyName">Name of the property to remove from the sort collection</param>
        public void RemoveSortEntry(string propertyName)
        {
            var removedFromAvailable = RemoveSortEntryFromCollection(AvailableSortEntries, propertyName);
            var removedFromSorted = RemoveSortEntryFromCollection(EntriesSortedBy, propertyName);

            if (removedFromAvailable == false && removedFromSorted == false)
            {
                throw new InvalidOperationException($"Property {propertyName} could not be found - Check that the provided name is valid and try again");
            }
        }

        /// <summary>
        /// Request the collection be sorted by a specific property.
        /// Note that this method will clear the existing sort.
        /// If the property is not in the known sort collection a debug message will be logged and sorting will not be executed.
        /// </summary>
        /// <param name="propertyToSortBy">Name of the property to sort by</param>
        public void RequestSort(string propertyToSortBy)
        {
            var currentlySortedEntries = EntriesSortedBy.ToList();
            EntriesSortedBy.Clear();
            currentlySortedEntries.ForEach(x => AvailableSortEntries.Add(x));

            var entryToSortBy = AvailableSortEntries.FirstOrDefault(x => x.PropertyName == propertyToSortBy);
            if (entryToSortBy == null)
            {
                throw new InvalidOperationException($"The property {propertyToSortBy} could not be found in the sort collection. Make sure it is attributed and that the requested name is correct");
            }

            AvailableSortEntries.Remove(entryToSortBy);
            EntriesSortedBy.Add(entryToSortBy);
        }

        /// <summary>
        /// Sort the collection by multiple entries.
        /// This method is designed to be used by views to pre-sort by multiple properties during initialization.
        /// Note that <see cref="GenerateSortEntries"/> must be called before this method is called
        /// </summary>
        /// <param name="propertiesToSortBy">The properties that the collection should be sorted by</param>
        public void SortByMultipleProperties(params string[] propertiesToSortBy)
        {
            foreach (var property in propertiesToSortBy)
            {
                var entry = AvailableSortEntries.FirstOrDefault(x => x.PropertyName == property);
                if (entry == null)
                {
                    throw new InvalidOperationException($"The property {property} could not be found in the sort collection");
                }

                AvailableSortEntries.Remove(entry);
                EntriesSortedBy.Add(entry);
            }
        }

        #endregion
    }
}