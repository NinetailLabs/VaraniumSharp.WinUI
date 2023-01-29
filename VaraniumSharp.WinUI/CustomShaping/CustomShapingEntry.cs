using System;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.CustomShaping;

/// <inheritdoc />
public class CustomShapingEntry : ShapingEntry
{
    #region Constructor

    public CustomShapingEntry(string entryType) 
        : base(entryType)
    { }

    #endregion

    #region Properties

    /// <summary>
    /// The data that defines the custom shaping
    /// </summary>
    public CustomShapingData? CustomData
    {
        get => _customData;
        set
        {
            if (_customData != null)
            {
                _customData.ShapingDataChanged -= CustomDataOnShapingDataChanged;
            }

            _customData = value;
            if (_customData != null)
            {
                _customData.ShapingDataChanged += CustomDataOnShapingDataChanged;
            }
        }
    }

    #endregion

    #region Private Methods

    private void CustomDataOnShapingDataChanged(object? sender, EventArgs e)
    {
        RequestShapingUpdateEvent();
    }

    #endregion

    #region Variables

    private CustomShapingData? _customData;

    #endregion
}