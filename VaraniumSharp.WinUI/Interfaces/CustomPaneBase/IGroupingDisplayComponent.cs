using System;
using System.Collections.Generic;
using VaraniumSharp.WinUI.GroupModule;

namespace VaraniumSharp.WinUI.Interfaces.CustomPaneBase
{
    /// <summary>
    /// A specialized <see cref="IDisplayComponent"/> that supports grouping with the <see cref="GroupingPropertyModule"/>
    /// </summary>
    public interface IGroupingDisplayComponent : IDisplayComponent
    {
        #region Events

        /// <summary>
        /// Occurs when the group order of the control has changed
        /// </summary>
        event EventHandler? GroupChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Module that is used to handle the grouping of components
        /// </summary>
        GroupingPropertyModule GroupingPropertyModule { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize the grouping of the list by property names
        /// </summary>
        /// <param name="groupEntries">Collection of entries to group by</param>
        void InitGroupOrder(List<GroupEntryStorageModel> groupEntries);

        #endregion
    }
}