using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using VaraniumSharp.WinUI.FilterModule.Controls;

namespace VaraniumSharp.WinUI.FilterModule
{
    /// <summary>
    /// Contains the methods used to create the controls
    /// </summary>
    public sealed partial class FilterablePropertyModule
    {
        #region Private Methods

        /// <summary>
        /// Create a boolean filter control and add it to the collection
        /// </summary>
        /// <param name="fullPropertyName">Full name of the property to filter on</param>
        /// <param name="attribute">Attribute used to get filter values</param>
        /// <param name="property">Property the filter is for</param>
        [FilterableControlCreation(FilterableType.Boolean)]
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Method is used via Reflection")]
        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Property is required for signature to match Action")]
        private void AddBooleanFilterControl(string fullPropertyName, FilterablePropertyAttribute attribute, PropertyInfo property)
        {
            if (!FilterAlreadyExists(attribute.FilterDisplayName))
            {
                var control = new DropDownBoolFilter(GetShapingEntry(fullPropertyName, attribute.FilterDisplayName, attribute.ToolTip));
                HookupFilterControl(control);
                
            }
        }

        /// <summary>
        /// Create a enumeration filter control and add it to the collection
        /// </summary>
        /// <param name="fullPropertyName">Full name of the property to filter on</param>
        /// <param name="attribute">Attribute used to get filter values</param>
        /// <param name="property">Property the filter is for</param>
        [FilterableControlCreation(FilterableType.Enumeration)]
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Method is used via Reflection")]
        private void AddEnumerationFilterControl(string fullPropertyName, FilterablePropertyAttribute attribute, PropertyInfo property)
        {
            var values = Enum.GetValues(property.PropertyType).Cast<object>().ToList();

            if (!FilterAlreadyExists(attribute.FilterDisplayName))
            {
                var shapingEntry = GetShapingEntry(fullPropertyName, attribute.FilterDisplayName, attribute.ToolTip);
                var control = new DropDownEnumFilter(shapingEntry, values);
                HookupFilterControl(control);
            }
        }

        /// <summary>
        /// Create a predefined string filter control and add it to the collection
        /// </summary>
        /// <param name="fullPropertyName">Full name of the property to filter on</param>
        /// <param name="attribute">Attribute used to get filter values</param>
        /// <param name="property">Property the filter is for</param>
        [FilterableControlCreation(FilterableType.PredefinedString)]
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Method is used via Reflection")]
        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Property is required for signature to match Action")]
        private void AddPredefinedStringFilterControl(string fullPropertyName, FilterablePropertyAttribute attribute, PropertyInfo property)
        {
            var type = Type.GetType(attribute.FilterCollectionClassName);
            var filterValues = (List<string>?)type?.GetProperty(attribute.FilterListPropertyName, BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
            if (filterValues == null)
            {
                throw new InvalidOperationException($"No pre-defined string filters could be found for {fullPropertyName}");
            }

            if (!FilterAlreadyExists(attribute.FilterDisplayName))
            {
                var shapingProperty = GetShapingEntry(fullPropertyName, attribute.Header, attribute.ToolTip);
                var control = new DropDownStringFilter(shapingProperty, filterValues);
                control.RefreshFiltering += ControlOnRefreshFiltering;
                HookupFilterControl(control);
            }
        }

        /// <summary>
        /// Create a searchable string filter control and add it to the collection
        /// </summary>
        /// <param name="fullPropertyName">Full name of the property to filter on</param>
        /// <param name="attribute">Attribute used to get filter values</param>
        /// <param name="property">Property the filter is for</param>
        [FilterableControlCreation(FilterableType.SearchableString)]
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Method is used via Reflection")]
        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Property is required for signature to match Action")]
        private void AddSearchableStringFilterControl(string fullPropertyName, FilterablePropertyAttribute attribute, PropertyInfo property)
        {
            if (!FilterAlreadyExists(attribute.FilterDisplayName))
            {
                var shapingEntry = GetShapingEntry(fullPropertyName, attribute.Header, attribute.ToolTip);
                var control = new StringFilter(shapingEntry);
                HookupFilterControl(control);
            }
        }

        /// <summary>
        /// Create a new shaping entry and populate it with the provided values
        /// </summary>
        /// <param name="fullPropertyName">Full property name the filter is for</param>
        /// <param name="header">Header to display for the filter control</param>
        /// <param name="tooltip">Tooltip to display for the filter control</param>
        /// <returns>Populated shaping entry</returns>
        private FilterShapingEntry GetShapingEntry(string fullPropertyName, string header, string tooltip)
        {
            return new FilterShapingEntry(string.Empty)
            {
                PropertyName = fullPropertyName,
                Header = header,
                Tooltip = tooltip
            };
        }

        #endregion
    }
}