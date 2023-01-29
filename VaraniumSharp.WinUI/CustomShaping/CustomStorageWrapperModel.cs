using System;
using System.Collections.Generic;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.CustomShaping;

/// <summary>
/// Stores the <see cref="CustomStorageModel"/> collection for a view
/// </summary>
public class CustomStorageWrapperModel : ShapingStorageWrapperModel<CustomStorageModel, CustomEntryStorageModel, CustomShapingEntry>
{
    #region Constructor

    /// <summary>
    /// Default Constructor
    /// </summary>
    public CustomStorageWrapperModel()
    { }

    /// <summary>
    /// Construct and populate
    /// </summary>
    /// <param name="layoutName">Name of the layout being stored</param>
    /// <param name="data">ShapingStorage models to store</param>
    public CustomStorageWrapperModel(Guid layoutName, List<CustomStorageModel> data)
        : base(layoutName, data)
    { }

    #endregion
}