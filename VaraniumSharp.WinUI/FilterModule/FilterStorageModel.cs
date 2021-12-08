using System;
using System.Collections.Generic;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.FilterModule
{
    /// <summary>
    /// Class used to store sort order for a control
    /// </summary>
    public class FilterStorageModel : ShapingStorageModelBase<FilterEntryStorageModel, FilterShapingEntry>
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FilterStorageModel()
        {
            SubEntries = new();
        }

        /// <summary>
        /// Constructor and populate
        /// </summary>
        /// <param name="instanceId">Instance id of the control the sort is for</param>
        /// <param name="filterShapingEntries">Entries that the control is filtered by</param>
        public FilterStorageModel(Guid instanceId, List<FilterShapingEntry> filterShapingEntries)
            : base(instanceId, filterShapingEntries)
        {
            SubEntries = new();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sub entries of the storage model.
        /// These entries are used for controls that are part of a sub layout pane
        /// </summary>
        public List<FilterStorageModel> SubEntries { get; set; }

        #endregion

        #region Private Methods

        /// <inheritdoc />
        protected override FilterEntryStorageModel CreateInstance(FilterShapingEntry shapingEntry)
        {
            return new FilterEntryStorageModel(shapingEntry);
        }

        #endregion
    }
}