using System.Collections.Generic;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Stores the <see cref="SortStorageModel"/> collection for a view
    /// </summary>
    public class SortStorageWrapperModel
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SortStorageWrapperModel()
        {
            SortStorage = new List<SortStorageModel>();
        }

        /// <summary>
        /// Sort collection to store
        /// </summary>
        public List<SortStorageModel> SortStorage { get; set; }
    }
}
