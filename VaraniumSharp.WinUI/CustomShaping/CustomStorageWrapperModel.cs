using System;
using System.Collections.Generic;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.CustomShaping;

public class CustomStorageWrapperModel : ShapingStorageWrapperModel<CustomStorageModel, CustomEntryStorageModel, CustomShapingEntry>
{
    #region Constructor

    public CustomStorageWrapperModel()
    { }

    public CustomStorageWrapperModel(Guid layoutName, List<CustomStorageModel> data)
        : base(layoutName, data)
    { }

    #endregion
}