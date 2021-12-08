using System;
using System.Collections.Generic;
using VaraniumSharp.WinUI.FilterModule;

namespace VaraniumSharp.WinUI.Interfaces.CustomPaneBase
{
    /// <summary>
    /// A specialized <see cref="IDisplayComponent"/> that supports filtering with the <see cref="FilterablePropertyModule"/>
    /// </summary>
    public interface IFilteringDisplayComponent : IDisplayComponent
    {
        #region Events

        /// <summary>
        /// Occurs when the filtering of the control has changed
        /// </summary>
        event EventHandler? FilterChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Module that is used to handle the filtering of entries
        /// </summary>
        FilterablePropertyModule FilterablePropertyModule { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize the filtering of the list
        /// </summary>
        /// <param name="filterEntries">Collection of entries to filter by</param>
        void InitFilterOrder(List<FilterEntryStorageModel> filterEntries);

        #endregion
    }
}