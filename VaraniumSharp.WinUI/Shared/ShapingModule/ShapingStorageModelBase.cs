using System;
using System.Collections.Generic;
using System.Linq;

namespace VaraniumSharp.WinUI.Shared.ShapingModule
{
    /// <summary>
    /// Class used to store shaping entries for a control
    /// </summary>
    public abstract class ShapingStorageModelBase<T, TD> where T : ShapingEntryStorageModelBase where TD: ShapingEntry
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        protected ShapingStorageModelBase()
        {
            ShapingEntries = new();
        }

        /// <summary>
        /// Construct and populate
        /// </summary>
        /// <param name="instanceId">Instance id of the control the shaping is for</param>
        /// <param name="shapingEntries">Entries that the control is shaping by</param>
        protected ShapingStorageModelBase(Guid instanceId, List<TD> shapingEntries)
        {
            InstanceId = instanceId;
            ShapingEntries = shapingEntries
                .Select(CreateInstance)
                .ToList();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The unique identifier of the control that the shaping is for
        /// </summary>
        public Guid InstanceId { get; set; }

        /// <summary>
        /// Shaping entries to store
        /// </summary>
        public List<T> ShapingEntries { get; set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method used to create a new instance of a <see cref="ShapingEntryStorageModelBase"/> populated by the <see cref="ShapingEntry"/>
        /// </summary>
        /// <param name="shapingEntry">The shaping entry to use for populating the entry</param>
        /// <returns>Populated storage model</returns>
        protected abstract T CreateInstance(TD shapingEntry);

        #endregion
    }
}