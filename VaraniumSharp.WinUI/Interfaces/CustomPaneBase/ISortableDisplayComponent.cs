using System;
using System.Collections.Generic;
using VaraniumSharp.WinUI.SortModule;

namespace VaraniumSharp.WinUI.Interfaces.CustomPaneBase
{
    /// <summary>
    /// A specialized <see cref="IDisplayComponent"/> that support sorting with the <see cref="SortablePropertyModule"/>
    /// </summary>
    public interface ISortableDisplayComponent : IDisplayComponent
    {
        /// <summary>
        /// Module that is used to handle the sorting of components
        /// </summary>
        SortablePropertyModule SortablePropertyModule { get; }

        /// <summary>
        /// Initialize sorting of the list by property names
        /// </summary>
        /// <param name="propertyNames">Names of the properties to sort by</param>
        void InitSortOrder(string[] propertyNames);

        /// <summary>
        /// Occurs when the sort order of the control has changed
        /// </summary>
        event EventHandler? SortChanged;
    }
}
