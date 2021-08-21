using System;
using System.Threading.Tasks;

namespace VaraniumSharp.WinUI.Interfaces.CustomPaneBase
{
    /// <summary>
    /// Assist with the routing of events for CustomPane layout details
    /// </summary>
    public interface ICustomLayoutEventRouter
    {
        #region Events

        /// <summary>
        /// Event that is fired when the <see cref="ShowControls"/> value changes
        /// </summary>
        event EventHandler<bool>? ControlDisplayChanged;

        /// <summary>
        /// Event that is fired when the layout of the controls changed
        /// </summary>
        event EventHandler? LayoutChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Indicate if Custom Layout management controls should be shown
        /// </summary>
        bool ShowControls { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the value indicating if the controls should display their management controls
        /// </summary>
        /// <param name="value">Value to set <see cref="ShowControls"/> to</param>
        Task SetControlDisplayValue(bool value);

        /// <summary>
        /// Fires the event to indicate that the layout of the controls has been changed
        /// </summary>
        Task SetLayoutChanged();

        #endregion
    }
}