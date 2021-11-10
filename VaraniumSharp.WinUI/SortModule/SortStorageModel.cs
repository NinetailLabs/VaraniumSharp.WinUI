using System;
using System.Collections.Generic;
using System.Linq;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Class used to store sort order for a control
    /// </summary>
    public class SortStorageModel
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SortStorageModel()
        {
            SortEntries = new List<SortEntryStorageModel>();
        }

        /// <summary>
        /// Construct and populate
        /// </summary>
        /// <param name="contentId">Guid of the control the sort is for</param>
        /// <param name="sortEntries">Entries that the control is sorted by</param>
        public SortStorageModel(Guid contentId, List<SortOrderEntry> sortEntries)
        {
            ContentId = contentId;
            SortEntries = sortEntries.Select(x => new SortEntryStorageModel(x)).ToList();
        }

        /// <summary>
        /// Content Id of the control that the sort is for
        /// </summary>
        public Guid ContentId { get; set; }

        /// <summary>
        /// Sort entries to store
        /// </summary>
        public List<SortEntryStorageModel> SortEntries { get; set; }
    }
}
