using System;
using System.ComponentModel;
using CommunityToolkit.WinUI.UI;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Attribute used to mark properties in a ViewModel as sortable
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SortablePropertyAttribute : Attribute
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        private SortablePropertyAttribute()
        {
            NestedTypes = Type.EmptyTypes;
            Header = string.Empty;
            ToolTip = string.Empty;
            DefaultSortDirection = SortDirection.Ascending;
        }

        /// <summary>
        /// Construct the attribute
        /// </summary>
        /// <param name="header">The title to display on the sort control</param>
        /// <param name="toolTip">Tooltip to display on the search button</param>
        public SortablePropertyAttribute(string header, string toolTip)
            : this()
        {
            Header = header;
            ToolTip = toolTip;
        }

        /// <summary>
        /// Constructor used for properties that contain nested sortable properties
        /// </summary>
        /// <param name="nestedTypes">The types that should be included in the sort</param>
        public SortablePropertyAttribute(params Type[] nestedTypes)
            : this()
        {
            HasNestedSorts = true;
            NestedTypes = nestedTypes;
        }

        /// <summary>
        /// Constructor used for properties that are generic and that contain nested sortable properties
        /// </summary>
        /// <param name="dictionaryIndex">Index in the module dictionary that contains the sort types</param>
        public SortablePropertyAttribute(int dictionaryIndex)
            : this()
        {
            HasNestedSorts = true;
            UseModuleDictionaryToGetSortTypes = true;
            ModuleDictionaryIndex = dictionaryIndex;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicate that the property is the default to use when sorting the collection
        /// </summary>
        public bool DefaultSort { get; set; }

        /// <summary>
        /// Gets or sets the default sort direction for the property
        /// </summary>
        public SortDirection DefaultSortDirection { get; set; }

        /// <summary>
        /// Indicate if the property is a complex containing properties that should also be sorted
        /// </summary>
        public bool HasNestedSorts { get; }

        /// <summary>
        /// Index of the dictionary entries that should be used to resolve the type.
        /// This should be used in conjunction with the <see cref="UseModuleDictionaryToGetSortTypes"/> property
        /// </summary>
        public int ModuleDictionaryIndex { get; }

        /// <summary>
        /// The typeof of the nested property - This is used because some types can be composed from multiple types
        /// </summary>
        public Type[] NestedTypes { get; }

        /// <summary>
        /// The text to display on the sort entry
        /// </summary>
        public string Header { get; }

        /// <summary>
        /// The tooltip to display on the sorting button
        /// </summary>
        public string ToolTip { get; }

        /// <summary>
        /// Indicate that the SortablePropertyModule should use its internal dictionary to resolve the property`s types.
        /// <remarks>
        /// This should not be used except on Properties that use a generic type
        /// </remarks>
        /// </summary>
        public bool UseModuleDictionaryToGetSortTypes { get; }

        #endregion
    }
}