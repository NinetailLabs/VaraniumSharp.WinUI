using System;

namespace VaraniumSharp.WinUI.Shared
{
    /// <summary>
    /// Base class for attributes that assist with live shaping of collections
    /// </summary>
    public abstract class ShapingPropertyAttributeBase : Attribute
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        private ShapingPropertyAttributeBase()
        {
            NestedTypes = Type.EmptyTypes;
            Header = string.Empty;
            ToolTip = string.Empty;
        }

        /// <summary>
        /// Construct the attribute
        /// </summary>
        /// <param name="header">The title to display on the shaping control</param>
        /// <param name="toolTip">Tooltip to display for the shaping control</param>
        protected ShapingPropertyAttributeBase(string header, string toolTip)
            : this()
        {
            Header = header;
            ToolTip = toolTip;
        }

        /// <summary>
        /// Constructor used for properties that contain nested shaping properties
        /// </summary>
        /// <param name="nestedTypes">The types that should be included</param>
        protected ShapingPropertyAttributeBase(params Type[] nestedTypes)
            : this()
        {
            HasNestedProperties = true;
            NestedTypes = nestedTypes;
        }

        /// <summary>
        /// Constructor used for properties that are generic and that contain nested shaping properties
        /// </summary>
        /// <param name="dictionaryIndex">Index in the module dictionary that contains the sort types</param>
        protected ShapingPropertyAttributeBase(int dictionaryIndex)
            : this()
        {
            HasNestedProperties = true;
            UseModuleDictionaryToGetSortTypes = true;
            ModuleDictionaryIndex = dictionaryIndex;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicate if the property is a complex containing properties that should also be shaped
        /// </summary>
        public bool HasNestedProperties { get; }

        /// <summary>
        /// The text to display on the sort entry
        /// </summary>
        public string Header { get; }

        /// <summary>
        /// Indicates that the property is the default to use when shaping the collection
        /// </summary>
        public bool IsDefault { get; set; }

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
        /// The tooltip to display for the shaping control
        /// </summary>
        public string ToolTip { get; }

        /// <summary>
        /// Indicate that the PropertyModule should use its internal dictionary to resolve the property`s types.
        /// <remarks>
        /// This should not be used except on Properties that use a generic type
        /// </remarks>
        /// </summary>
        public bool UseModuleDictionaryToGetSortTypes { get; }

        #endregion
    }
}