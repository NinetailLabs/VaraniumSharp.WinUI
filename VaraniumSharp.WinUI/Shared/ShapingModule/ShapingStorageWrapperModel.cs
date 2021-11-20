using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VaraniumSharp.WinUI.Shared.ShapingModule
{
    /// <summary>
    /// Stores the shaping storage model collection for a view
    /// </summary>
    public abstract class ShapingStorageWrapperModel<T, TD, TX> where T: ShapingStorageModelBase<TD, TX> where TD : ShapingEntryStorageModelBase where TX: ShapingEntry
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        protected ShapingStorageWrapperModel()
        {
            SortStorage = new();
        }

        /// <summary>
        /// Construct and populate
        /// </summary>
        /// <param name="layoutName">Name of the layout being stored</param>
        /// <param name="storageModels">SortStorage models to store</param>
        protected ShapingStorageWrapperModel(Guid layoutName, List<T> storageModels)
        {
            LayoutName = layoutName;
            SortStorage = storageModels;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the layout being stored
        /// </summary>
        [JsonInclude]
        public Guid LayoutName { get; set; }

        /// <summary>
        /// Sort collection to store
        /// </summary>
        [JsonInclude]
        public List<T> SortStorage { get; set; }

        #endregion
    }
}