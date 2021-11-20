using System;
using System.Collections.Generic;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Stores the <see cref="SortStorageModel"/> collection for a view
    /// </summary>
    public class SortStorageWrapperModel : ShapingStorageWrapperModel<SortStorageModel, SortEntryStorageModel,
        SortableShapingEntry>
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SortStorageWrapperModel()
        { }

        /// <summary>
        /// Construct and populate
        /// </summary>
        /// <param name="layoutName">Name of the layout being stored</param>
        /// <param name="storageModels">ShapingStorage models to store</param>
        public SortStorageWrapperModel(Guid layoutName, List<SortStorageModel> storageModels)
            : base(layoutName, storageModels)
        { }

        #endregion
    }
}
