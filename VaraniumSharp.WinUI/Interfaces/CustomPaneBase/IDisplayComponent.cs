using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace VaraniumSharp.WinUI.Interfaces.CustomPaneBase
{
    /// <summary>
    /// Interface for user controls
    /// </summary>
    public interface IDisplayComponent : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Internal Content Id of the control - Used to identify the control for loading
        /// </summary>
        Guid ContentId { get; }

        /// <summary>
        /// Unique Id for the instance of the control.
        /// This can be used to associated additional stored information with the correct instance during control initialization.
        /// </summary>
        Guid InstanceId { get; set; }

        /// <summary>
        /// The height of the control
        /// </summary>
        double Height { get; set; }

        /// <summary>
        /// Indicate if the resize handle for the control should be displayed
        /// </summary>
        public bool ShowResizeHandle { get; set; }

        /// <summary>
        /// Indicate if current load is part of ACM startup
        /// </summary>
        bool StartupLoad { get; set; }

        /// <summary>
        /// The title for the control
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// The width of the control
        /// </summary>
        double Width { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Method is guaranteed to be called only once during ACM startup so heavy/once off operations should be done here
        /// </summary>
        Task InitAsync();

        /// <summary>
        /// Sets the size of the control based on its parent's size
        /// </summary>
        /// <param name="width">Width to set</param>
        /// <param name="height">Height to set</param>
        Task SetControlSizeAsync(double width, double height);

        #endregion
    }
}