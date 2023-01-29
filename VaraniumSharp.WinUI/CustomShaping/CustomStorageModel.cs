using System;
using System.Collections.Generic;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.CustomShaping;

/// <summary>
/// Class used to store a collection of <see cref="CustomEntryStorageModel"/>
/// </summary>
public class CustomStorageModel : ShapingStorageModelBase<CustomEntryStorageModel, CustomShapingEntry>
{
    #region Constructor

    /// <summary>
    /// Default Constructor
    /// </summary>
    public CustomStorageModel()
    {
        SubEntries = new();
    }

    /// <summary>
    /// Construct and populate the storage model data
    /// </summary>
    /// <param name="instanceId">Instance Id of the control the data is stored for</param>
    /// <param name="customDataShapingEntries">Collection of custom data storage entries to persist</param>
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

    /// <inheritdoc />
    protected override CustomEntryStorageModel CreateInstance(CustomShapingEntry shapingEntry)
    {
        return new CustomEntryStorageModel(shapingEntry);
    }

    #endregion
}