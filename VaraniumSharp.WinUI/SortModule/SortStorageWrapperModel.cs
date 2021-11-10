using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
        /// Construct and populate
        /// </summary>
        /// <param name="layoutName">Name of the layout being stord</param>
        /// <param name="storageModels">SortStorage models to store</param>
        public SortStorageWrapperModel(Guid layoutName, List<SortStorageModel> storageModels)
        {
            LayoutName = layoutName;
            SortStorage = storageModels;
        }

        /// <summary>
        /// The name of the layout being stored
        /// </summary>
        [JsonInclude]
        public Guid LayoutName { get; set; }

        /// <summary>
        /// Sort collection to store
        /// </summary>
        [JsonInclude]
        public List<SortStorageModel> SortStorage { get; set; }
    }
}
