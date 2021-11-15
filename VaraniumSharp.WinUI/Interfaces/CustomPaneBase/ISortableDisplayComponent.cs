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
        #region Events

        /// <summary>
        /// Occurs when the sort order of the control has changed
        /// </summary>
        event EventHandler? SortChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Module that is used to handle the sorting of components
        /// </summary>
        SortablePropertyModule SortablePropertyModule { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize sorting of the list by property names
        /// </summary>
        /// <param name="sortEntries">Collection of entries to sort by</param>
        void InitSortOrder(List<SortEntryStorageModel> sortEntries);

        #endregion
    }
}
