using System;
using System.Collections.Generic;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.FilterModule
{
    /// <summary>
    /// Stores the <see cref="FilterStorageModel"/> collection for a view
    /// </summary>
    public class FilterStorageWrapperModel : ShapingStorageWrapperModel<FilterStorageModel, FilterEntryStorageModel, FilterShapingEntry>
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FilterStorageWrapperModel()
        { }

        /// <summary>
        /// Construct and populate
        /// </summary>
        /// <param name="layoutName">Name of the layout being stored</param>
        /// <param name="storageModels">ShapingStorage models to store</param>
        public FilterStorageWrapperModel(Guid layoutName, List<FilterStorageModel> storageModels)
            : base(layoutName, storageModels)
        { }

        #endregion
    }
}