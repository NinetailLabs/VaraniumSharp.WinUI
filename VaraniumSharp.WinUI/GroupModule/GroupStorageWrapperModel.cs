using System;
using System.Collections.Generic;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.GroupModule
{
    /// <summary>
    /// Stores the <see cref="GroupStorageModel"/> collection for a view
    /// </summary>
    public class GroupStorageWrapperModel : ShapingStorageWrapperModel<GroupStorageModel, GroupEntryStorageModel, GroupShapingEntry>
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public GroupStorageWrapperModel()
        { }

        /// <summary>
        /// Construct and populate
        /// </summary>
        /// <param name="layoutName">Name of the layout being stored</param>
        /// <param name="storageModels">ShapingStorage models to store</param>
        public GroupStorageWrapperModel(Guid layoutName, List<GroupStorageModel> storageModels)
            : base(layoutName, storageModels)
        { }

        #endregion
    }
}