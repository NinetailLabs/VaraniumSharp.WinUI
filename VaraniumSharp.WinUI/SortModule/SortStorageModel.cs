using System;
using System.Collections.Generic;
using System.Linq;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Class used to store sort order for a control
    /// </summary>
    public class SortStorageModel
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SortStorageModel()
        {
            SortEntries = new();
            SubEntries = new();
        }

        /// <summary>
        /// Construct and populate
        /// </summary>
        /// <param name="instanceId">Instance id of the control the sort is for</param>
        /// <param name="sortEntries">Entries that the control is sorted by</param>
        public SortStorageModel(Guid instanceId, List<ShapingEntry> sortEntries)
        {
            InstanceId = instanceId;
            SortEntries = sortEntries
                .Select(x => new SortEntryStorageModel(x as SortableShapingEntry))
                .ToList();
            SubEntries = new();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The unqiue identifier of the control that the sort is for
        /// </summary>
        public Guid InstanceId { get; set; }

        /// <summary>
        /// Sort entries to store
        /// </summary>
        public List<SortEntryStorageModel> SortEntries { get; set; }

        /// <summary>
        /// Sub entries of the storage model.
        /// These entries are used for controls that are part of a sub layout pane
        /// </summary>
        public List<SortStorageModel> SubEntries { get; set; }

        #endregion
    }
}
