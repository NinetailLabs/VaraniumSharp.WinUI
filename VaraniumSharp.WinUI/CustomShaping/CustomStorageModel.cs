using System;
using System.Collections.Generic;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.CustomShaping;

public class CustomStorageModel : ShapingStorageModelBase<CustomEntryStorageModel, CustomShapingEntry>
{
    #region Constructor

    public CustomStorageModel()
    {
        SubEntries = new();
    }

    public CustomStorageModel(Guid instanceId, List<CustomShapingEntry> customDataShapingEntries)
        : base(instanceId, customDataShapingEntries)
    {
        SubEntries = new();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Sub entries of the storage model.
    /// These entries are used for controls that are part of a sub layout pane
    /// </summary>
    public List<CustomStorageModel> SubEntries { get; set; }

    #endregion

    #region Private Methods

    protected override CustomEntryStorageModel CreateInstance(CustomShapingEntry shapingEntry)
    {
        return new CustomEntryStorageModel(shapingEntry);
    }

    #endregion
}