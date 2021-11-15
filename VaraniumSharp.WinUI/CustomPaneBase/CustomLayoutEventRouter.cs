using System;
using System.Threading.Tasks;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;

namespace VaraniumSharp.WinUI.CustomPaneBase
{
    /// <summary>
    /// Assist with the routing of events for CustomPane layout details
    /// </summary>
    [AutomaticContainerRegistration(typeof(ICustomLayoutEventRouter), VaraniumSharp.Enumerations.ServiceReuse.Singleton)]
    public class CustomLayoutEventRouter : ICustomLayoutEventRouter
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public CustomLayoutEventRouter()
        {
            ShowControls = false;
        }

        #endregion

        #region Events

        /// <inheritdoc/>
        public event EventHandler<bool>? ControlDisplayChanged;

        /// <inheritdoc/>
        public event EventHandler? LayoutChanged;

        /// <inheritdoc/>
        public event EventHandler? SortChanged;

        #endregion

        #region Properties

        /// <inheritdoc/>
        public bool ShowControls { get; private set; }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public Task SetControlDisplayValue(bool value)
        {
            ShowControls = value;
            ControlDisplayChanged?.Invoke(this, value);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task SetLayoutChanged()
        {
            LayoutChanged?.Invoke(this, EventArgs.Empty);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void SetSortOrderChanged()
        {
            SortChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}