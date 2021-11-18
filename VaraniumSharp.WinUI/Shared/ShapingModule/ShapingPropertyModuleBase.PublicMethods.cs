using System;
using System.Linq;

namespace VaraniumSharp.WinUI.Shared.ShapingModule
{
    /// <summary>
    /// Public methods of the ShapingPropertyModuleBase
    /// </summary>
    public abstract partial class ShapingPropertyModuleBase<T>
    {
        #region Public Methods

        /// <summary>
        /// Occurs when the user clicks the shaping clear button.
        /// Will clear all current sorts.
        /// </summary>
        public void ClearShapingOnClick()
        {
            var entriesToRemove = EntriesShapedBy.ToList();
            foreach (var sortOrderData in entriesToRemove)
            {
                AvailableShapingEntries.Add(sortOrderData);
                EntriesShapedBy.Remove(sortOrderData);
            }
        }

        /// <summary>
        /// Generate the buttons for shaping the collection
        /// </summary>
        /// <param name="collectionTypes">
        /// Type(s) of entries in the collection.
        /// Multiple types are allowed in case the interface implements another interface(s) so that those can also contribute sorting properties
        /// </param>
        public void GenerateShapingEntries(params Type[] collectionTypes)
        {
            foreach (var collectionType in collectionTypes)
            {
                var sortableProperties = GetPropertiesForType(collectionType);
                HandleShapingEntryGeneration(string.Empty, sortableProperties);
            }

            if (!string.IsNullOrEmpty(DefaultShapingProperty)
                && !DisableDefaultShaping)
            {
                var availableEntry = AvailableShapingEntries.First(x => x.PropertyName == DefaultShapingProperty);
                AvailableShapingEntries.Remove(availableEntry);
                EntriesShapedBy.Add(availableEntry);
            }
        }

        /// <summary>
        /// Move the <see cref="SelectedAvailableEntry"/> to the <see cref="EntriesShapedBy"/> collection to shape the collection.
        /// Note that the entry will be added to the end.
        /// </summary>
        public void MoveEntryFromAvailableToShapedBy()
        {
            var entry = SelectedAvailableEntry;
            if (entry != null)
            {
                AvailableShapingEntries.Remove(entry);
                EntriesShapedBy.Add(entry);
            }
        }

        /// <summary>
        /// Move the <see cref="SelectedShapedByEntry"/> to the <see cref="AvailableShapingEntries"/> collection to shape the collection.
        /// Note that the entry will be added to the end.
        /// </summary>
        public void MoveEntryFromShapedByToAvailable()
        {
            var entry = SelectedShapedByEntry;
            if (entry != null)
            {
                EntriesShapedBy.Remove(entry);
                AvailableShapingEntries.Add(entry);
            }
        }

        /// <summary>
        /// Removes a shaping property using the name of the property
        /// </summary>
        /// <param name="propertyName">Name of the property to remove from the shaping collection</param>
        public void RemoveShapingEntry(string propertyName)
        {
            var removedFromAvailable = RemoveShapingEntryFromCollection(AvailableShapingEntries, propertyName);
            var removedFromSorted = RemoveShapingEntryFromCollection(EntriesShapedBy, propertyName);

            if (!removedFromAvailable && !removedFromSorted)
            {
                throw new InvalidOperationException($"Property {propertyName} could not be found - Check that the provided name is valid and try again");
            }
        }

        /// <summary>
        /// Request the collection be shaped by a specific property.
        /// Note that this method will clear the existing shaping.
        /// If the property is not in the known sort collection a debug message will be logged and sorting will not be executed.
        /// </summary>
        /// <param name="propertyToSortBy">Name of the property to shape by</param>
        public void RequestShaping(string propertyToSortBy)
        {
            var currentlySortedEntries = EntriesShapedBy.ToList();
            EntriesShapedBy.Clear();
            currentlySortedEntries.ForEach(x => AvailableShapingEntries.Add(x));

            var entryToSortBy = AvailableShapingEntries.FirstOrDefault(x => x.PropertyName == propertyToSortBy);
            if (entryToSortBy == null)
            {
                throw new InvalidOperationException($"The property {propertyToSortBy} could not be found in the shaping collection. Make sure it is attributed and that the requested name is correct");
            }

            AvailableShapingEntries.Remove(entryToSortBy);
            EntriesShapedBy.Add(entryToSortBy);
        }

        /// <summary>
        /// Shape the collection by multiple entries.
        /// This method is designed to be used by views to pre-sort by multiple properties during initialization.
        /// Note that <see cref="GenerateShapingEntries"/> must be called before this method is called
        /// </summary>
        /// <param name="propertiesToSortBy">The properties that the collection should be sorted by</param>
        public void ShapeByMultipleProperties(params string[] propertiesToSortBy)
        {
            foreach (var property in propertiesToSortBy)
            {
                var entry = AvailableShapingEntries.FirstOrDefault(x => x.PropertyName == property);
                if (entry == null)
                {
                    throw new InvalidOperationException($"The property {property} could not be found in the shaping collection");
                }

                AvailableShapingEntries.Remove(entry);
                EntriesShapedBy.Add(entry);
            }
        }

        #endregion
    }
}