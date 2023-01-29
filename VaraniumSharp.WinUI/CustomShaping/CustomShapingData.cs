using System;

namespace VaraniumSharp.WinUI.CustomShaping;

/// <summary>
/// The data used to persist custom shaping entries
/// </summary>
public class CustomShapingData
{
    #region Constructor

    /// <summary>
    /// Default Constructor
    /// </summary>
    public CustomShapingData()
    {
        _customDataJson = "{}";
    }

    #endregion

    #region Events

    /// <summary>
    /// Fired when the shaping data has changed
    /// </summary>
    public event EventHandler? ShapingDataChanged;

    #endregion

    #region Properties

    /// <summary>
    /// Custom data stored as a Json string
    /// </summary>
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

    /// <summary>
    /// Backing variable for <see cref="CustomDataJson"/> property
    /// </summary>
    private string _customDataJson;

    #endregion
}