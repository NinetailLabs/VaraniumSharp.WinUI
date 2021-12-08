using System;

namespace VaraniumSharp.WinUI.FilterModule
{
    /// <summary>
    /// Attribute used to indicate that a method provides a filterable control
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class FilterableControlCreationAttribute : Attribute
    {
        #region Constructor

        /// <summary>
        /// Construct with filter type
        /// </summary>
        /// <param name="filterType">Type of filter the method provides a control for</param>
        public FilterableControlCreationAttribute(FilterableType filterType)
        {
            FilterType = filterType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Type of filter the method proves a control for
        /// </summary>
        public FilterableType FilterType { get; }

        #endregion
    }
}