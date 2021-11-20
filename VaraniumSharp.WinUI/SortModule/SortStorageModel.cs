using System;
using System.Collections.Generic;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Class used to store sort order for a control
    /// </summary>
    public class SortStorageModel : ShapingStorageModelBase<SortEntryStorageModel, SortableShapingEntry>
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SortStorageModel()
        {
            SubEntries = new();
        }

        /// <summary>
        /// Construct and populate
        /// </summary>
        /// <param name="instanceId">Instance id of the control the sort is for</param>
        /// <param name="sortableShapingEntries">Entries that the control is sorted by</param>
        public SortStorageModel(Guid instanceId, List<SortableShapingEntry> sortableShapingEntries)
            : base(instanceId, sortableShapingEntries)
        {
            SubEntries = new();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sub entries of the storage model.
        /// These entries are used for controls that are part of a sub layout pane
        /// </summary>
        public List<SortStorageModel> SubEntries { get; set; }

        #endregion

        #region Private Methods

        /// <inheritdoc />
        protected override SortEntryStorageModel CreateInstance(SortableShapingEntry shapingEntry)
        {
            return new SortEntryStorageModel(shapingEntry);
        }

        #endregion
    }
}
