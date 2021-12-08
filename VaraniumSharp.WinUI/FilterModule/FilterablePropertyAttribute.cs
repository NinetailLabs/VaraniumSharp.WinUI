using System;
using System.Collections.Generic;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.FilterModule
{
    /// <summary>
    /// Attribute used to mark properties in a ViewModel as filterable
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FilterablePropertyAttribute : ShapingPropertyAttributeBase
    {
        #region Constructor
        
        /// <summary>
        /// Construct with a tooltip and preferred button index
        /// </summary>
        /// <param name="filterDisplayName">The name to display for the filter control</param>
        /// <param name="tooltip">Tooltip to display on the filter control</param>
        /// <param name="filterType">The type of filter control that should be created</param>
        /// <param name="preferredButtonIndex">Index of the filter control</param>
        public FilterablePropertyAttribute(string filterDisplayName, string tooltip, FilterableType filterType, int preferredButtonIndex)
            : base(filterDisplayName, tooltip)
        {
            FilterCollectionClassName = string.Empty;
            FilterListPropertyName = string.Empty;
            PreferredButtonIndex = preferredButtonIndex;
            FilterDisplayName = filterDisplayName;
            FilterType = filterType;
        }

        /// <summary>
        /// Construct with a tooltip and preferred button index
        /// </summary>
        /// <param name="filterDisplayName">The name to display for the filter control</param>
        /// <param name="tooltip">Tooltip to display on the filter control</param>
        /// <param name="filterType">The type of filter control that should be created</param>
        /// <param name="preferredButtonIndex">Index of the filter control</param>
        /// <param name="className">The static class containing the list</param>
        /// <param name="filterListProperty">The name of the property that contains the filter values. The property must be a <see cref="List{T}"/> of <see cref="string"/>s</param>
        public FilterablePropertyAttribute(string filterDisplayName, string tooltip, FilterableType filterType, int preferredButtonIndex, string className, string filterListProperty)
            : this(filterDisplayName, tooltip, filterType, preferredButtonIndex)
        {
            FilterCollectionClassName = className;
            FilterListPropertyName = filterListProperty;
        }

        /// <summary>
        /// Constructor used for properties that contain nested filterable properties
        /// </summary>
        /// <param name="preferredButtonIndex">Index of the nested filter controls</param>
        /// <param name="nestedTypes">The types that should be included in the filter</param>
        public FilterablePropertyAttribute(int preferredButtonIndex, params Type[] nestedTypes)
            : base(nestedTypes)
        {
            FilterCollectionClassName = string.Empty;
            FilterListPropertyName = string.Empty;
            FilterDisplayName = string.Empty;
            HasNestedFilters = true;
            PreferredButtonIndex = preferredButtonIndex;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of the static class that contains the string to filter on
        /// </summary>
        public string FilterCollectionClassName { get; }

        /// <summary>
        /// The name to display in the filter control list
        /// </summary>
        public string FilterDisplayName { get; }

        /// <summary>
        /// The name of the property that contains the filter values. The property must be a <see cref="List{T}"/> of <see cref="string"/>s
        /// </summary>
        public string FilterListPropertyName { get; }

        /// <summary>
        /// The kind of filter control that should be created
        /// </summary>
        public FilterableType FilterType { get; }

        /// <summary>
        /// Indicates if the property is a complex one containing properties that should also be filtered
        /// </summary>
        public bool HasNestedFilters { get; }
        
        /// <summary>
        /// The index where the control should be placed
        /// </summary>
        public int PreferredButtonIndex { get; }

        #endregion
    }
}