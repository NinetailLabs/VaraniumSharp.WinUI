using System;
using CommunityToolkit.WinUI.UI;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Attribute used to mark properties in a ViewModel as sortable
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SortablePropertyAttribute : ShapingPropertyAttributeBase
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        private SortablePropertyAttribute()
        {
            DefaultSortDirection = SortDirection.Ascending;
        }

        /// <summary>
        /// Construct the attribute
        /// </summary>
        /// <param name="header">The title to display on the sort control</param>
        /// <param name="toolTip">Tooltip to display on the search button</param>
        public SortablePropertyAttribute(string header, string toolTip)
            : base(header, toolTip)
        {
            DefaultSortDirection = SortDirection.Ascending;
        }

        /// <summary>
        /// Constructor used for properties that contain nested sortable properties
        /// </summary>
        /// <param name="nestedTypes">The types that should be included in the sort</param>
        public SortablePropertyAttribute(params Type[] nestedTypes)
            : base(nestedTypes)
        {
            DefaultSortDirection = SortDirection.Ascending;
        }

        /// <summary>
        /// Constructor used for properties that are generic and that contain nested sortable properties
        /// </summary>
        /// <param name="dictionaryIndex">Index in the module dictionary that contains the sort types</param>
        public SortablePropertyAttribute(int dictionaryIndex)
            : base(dictionaryIndex)
        {
            DefaultSortDirection = SortDirection.Ascending;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the default sort direction for the property
        /// </summary>
        public SortDirection DefaultSortDirection { get; set; }

        #endregion
    }
}