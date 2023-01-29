using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.CustomShaping;

/// <summary>
/// Storage model used to store <see cref="CustomShapingEntry"/> data
/// </summary>
public class CustomEntryStorageModel : ShapingEntryStorageModelBase
{
    #region Constructor

    /// <summary>
    /// Default Constructor
    /// </summary>
    public CustomEntryStorageModel()
    { }

    /// <summary>
    /// Construct and populate with data
    /// </summary>
    /// <param name="customData">The data to populate the storage model with</param>
    public CustomEntryStorageModel(CustomShapingEntry customData)
        : base(customData)
    {
        CustomData = customData.CustomData;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The custom data stored by the model
    /// </summary>
    public CustomShapingData? CustomData { get; set; }

    #endregion
}