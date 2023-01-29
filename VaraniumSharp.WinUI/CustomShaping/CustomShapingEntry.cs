using System;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.CustomShaping;

/// <inheritdoc />
public class CustomShapingEntry : ShapingEntry
{
    #region Constructor

    /// <summary>
    /// Construct and set the type of the entry
    /// </summary>
    /// <param name="entryType">The type of shaping entry</param>
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

    /// <summary>
    /// Requests that the shaping data update event is fired
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event arguments</param>
    private void CustomDataOnShapingDataChanged(object? sender, EventArgs e)
    {
        RequestShapingUpdateEvent();
    }

    #endregion

    #region Variables

    /// <summary>
    /// Backing variable for the <see cref="CustomData"/> property
    /// </summary>
    private CustomShapingData? _customData;

    #endregion
}