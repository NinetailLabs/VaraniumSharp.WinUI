using System;

namespace VaraniumSharp.WinUI.CustomShaping;

public class CustomShapingData
{
    #region Events

    public event EventHandler? ShapingDataChanged;

    #endregion

    #region Properties

    public string CustomDataJson
    {
        get => _customDataJson;
        set
        {
            _customDataJson = value;
            ShapingDataChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion

    #region Variables

    private string _customDataJson;

    #endregion
}