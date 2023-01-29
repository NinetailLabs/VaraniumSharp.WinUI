using System;
using System.Collections.Generic;
using VaraniumSharp.WinUI.CustomShaping;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.Interfaces.CustomShaping;

public interface ICustomShapingDisplayComponent : IDisplayComponent
{
    #region Events

    /// <summary>
    /// Occurs when the filtering of the control has changed
    /// </summary>
    event EventHandler? ShapingChanged;

    #endregion

    #region Properties

    /// <summary>
    /// Collection containing entries that are currently used to shape the collection
    /// </summary>
    List<ShapingEntry> EntriesShapedBy { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initialize the filtering of the list
    /// </summary>
    /// <param name="filterEntries">Collection of entries to filter by</param>
    void InitFilterOrder(List<CustomEntryStorageModel> filterEntries);

    #endregion
}