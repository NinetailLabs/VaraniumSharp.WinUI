using System;
using VaraniumSharp.WinUI.Shared;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.GroupModule
{
    /// <summary>
    /// Attribute that is used to indicate that the property can be used for grouping
    /// </summary>
    public class GroupingPropertyAttribute : ShapingPropertyAttributeBase
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        private GroupingPropertyAttribute()
        { }

        /// <summary>
        /// Construct the attribute
        /// </summary>
        /// <param name="header">The title to display on the sort control</param>
        /// <param name="toolTip">Tooltip to display on the search button</param>
        public GroupingPropertyAttribute(string header, string toolTip)
            : base(header, toolTip)
        {
        }

        /// <summary>
        /// Constructor used for properties that contain nested sortable properties
        /// </summary>
        /// <param name="nestedTypes">The types that should be included in the sort</param>
        public GroupingPropertyAttribute(params Type[] nestedTypes)
            : base(nestedTypes)
        {
        }

        /// <summary>
        /// Constructor used for properties that are generic and that contain nested sortable properties
        /// </summary>
        /// <param name="dictionaryIndex">Index in the module dictionary that contains the sort types</param>
        public GroupingPropertyAttribute(int dictionaryIndex)
            : base(dictionaryIndex)
        {
        }

        #endregion
    }
}