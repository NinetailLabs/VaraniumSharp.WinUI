using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace VaraniumSharp.WinUI.Shared.ShapingModule
{
    public abstract partial class ShapingPropertyModuleBase<T>
    {
        #region Private Methods

        /// <summary>
        /// Create a button for a Property
        /// </summary>
        /// <param name="propertyName">Name of the property the button is for</param>
        /// <param name="attribute">Attribute containing the details of the shaping entry</param>
        /// <param name="propertyInfo">Property info for the shaping entry</param>
        protected virtual ShapingEntry? CreateShapingEntry(string propertyName, ShapingPropertyAttributeBase attribute, PropertyInfo propertyInfo)
        {
            return null;
        }

        /// <summary>
        /// Occurs when the <see cref="EntriesShapedBy"/> collection changed and is used to trigger a sort operation
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        protected abstract void EntriesShapedByOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e);

        /// <summary>
        /// Occurs when one of the shaping entries request reshaping of the collection
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        /// <exception cref="NotImplementedException"></exception>
        private void EntryOnRequestShapingUpdate(object? sender, EventArgs e)
        {
            if (sender is ShapingEntry sortOrder && EntriesShapedBy.Contains(sortOrder))
            {
                Shape(sortOrder.PropertyName);
            }
        }

        /// <summary>
        /// Request firing of the <see cref="ShapingChanged"/> event
        /// </summary>
        protected void FireShapingChangedEvent()
        {
            ShapingChanged?.Invoke(this, EventArgs.Empty);

        }

        /// <summary>
        /// Get the properties that are decorated with <see cref="ShapingPropertyAttributeBase"/> for a type
        /// </summary>
        /// <param name="type">Type properties should be retrieved for</param>
        /// <returns>Matching properties</returns>
        private static IEnumerable<PropertyInfo> GetPropertiesForType(Type type)
        {
            return type
                .GetProperties()
                .Where(x => x.GetCustomAttributes(typeof(ShapingPropertyAttributeBase), true).Any());
        }

        /// <summary>
        /// Handle the generation of sort entries for nested attributes
        /// </summary>
        /// <param name="attribute">Attribute for which sort entries should be created</param>
        /// <param name="propertyPrefix">Prefix to get to the attribute</param>
        /// <param name="property">Property info for the attribute</param>
        private void HandleNestedAttributeEntryGeneration(ShapingPropertyAttributeBase attribute, string propertyPrefix, PropertyInfo property)
        {
            var nestedTypes = attribute.UseModuleDictionaryToGetSortTypes
                        ? NestedTypeList[attribute.ModuleDictionaryIndex]
                        : attribute.NestedTypes;

            foreach (var attributeNestedType in nestedTypes)
            {
                var nestedProperties = GetPropertiesForType(attributeNestedType);
                HandleShapingEntryGeneration($"{propertyPrefix}{property.Name}.", nestedProperties);
            }
        }

        /// <summary>
        /// Generate buttons for collection shaping.
        /// This method includes logic to generate buttons for complex nested types
        /// </summary>
        /// <param name="propertyPrefix">Prefix of property for nested types</param>
        /// <param name="propertiesToCreateButtonsFor">Properties that buttons should be created for</param>
        private void HandleShapingEntryGeneration(string propertyPrefix, IEnumerable<PropertyInfo> propertiesToCreateButtonsFor)
        {
            propertiesToCreateButtonsFor = propertiesToCreateButtonsFor
                .OrderBy(x => ((T?)x.GetCustomAttribute(typeof(T)))?.Header);

            foreach (var property in propertiesToCreateButtonsFor)
            {
                var attribute = (T?)property.GetCustomAttribute(typeof(T));

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
                    var entry = CreateShapingEntry(fullPropertyName, attribute, property);
                    if (entry == null)
                    {
                        continue;
                    }

                    AvailableShapingEntries.Add(entry);
                    entry.RequestShapingUpdate += EntryOnRequestShapingUpdate;
                    
                    if (attribute.IsDefault && string.IsNullOrEmpty(DefaultShapingProperty))
                    {
                        DefaultShapingProperty = fullPropertyName;
                    }
                }
            }
        }

        /// <summary>
        /// Remove a shaping entry from the collection
        /// </summary>
        /// <param name="collection">Collection the entry should be removed from</param>
        /// <param name="propertyName">Property name of the entry to remove</param>
        /// <returns>Indicate if the entry could be found in the collection</returns>
        private static bool RemoveShapingEntryFromCollection(ObservableCollection<ShapingEntry> collection, string propertyName)
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
        /// Shape the collection based on a property
        /// </summary>
        /// <param name="propertyName">Name of the property that sorting should be done on</param>
        protected abstract void Shape(string propertyName);

        #endregion
    }
}