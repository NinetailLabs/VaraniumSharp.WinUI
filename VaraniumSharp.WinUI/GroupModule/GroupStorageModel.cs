using System;
using System.Collections.Generic;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.GroupModule
{
    /// <summary>
    /// Class used to store group order for a control
    /// </summary>
    public class GroupStorageModel : ShapingStorageModelBase<GroupEntryStorageModel, GroupShapingEntry>
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public GroupStorageModel()
        {
            SubEntries = new();
        }

        /// <summary>
        /// Construct and populate
        /// </summary>
        /// <param name="instanceId">Instance Id of the control the sort is for</param>
        /// <param name="groupShapingEntries">Entries that the control is sorted by</param>
        public GroupStorageModel(Guid instanceId, List<GroupShapingEntry> groupShapingEntries)
            : base(instanceId, groupShapingEntries) 
        { }

        #endregion

        #region Properties

        /// <summary>
        /// /Sub entries for the storage model.
        /// These entries are used for controls that are part of a sub layout.
        /// </summary>
        public List<GroupStorageModel> SubEntries { get; set; }

        #endregion

        #region Private Methods

        /// <inheritdoc />
        protected override GroupEntryStorageModel CreateInstance(GroupShapingEntry shapingEntry)
        {
            return new GroupEntryStorageModel(shapingEntry);
        }

        #endregion
    }
}