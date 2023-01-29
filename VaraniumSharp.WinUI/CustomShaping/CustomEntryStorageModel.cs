using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.CustomShaping;

public class CustomEntryStorageModel : ShapingEntryStorageModelBase
{
    #region Constructor

    public CustomEntryStorageModel()
    {

    }

    public CustomEntryStorageModel(CustomShapingEntry customData)
        : base(customData)
    {
        CustomData = customData.CustomData;
    }

    #endregion

    #region Properties

    public CustomShapingData? CustomData { get; set; }

    #endregion
}