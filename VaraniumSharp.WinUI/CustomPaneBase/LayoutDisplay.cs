using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.Dialogs;

namespace VaraniumSharp.WinUI.CustomPaneBase
{
    /// <summary>
    /// Assist the CustomPane with showing controls
    /// </summary>
    public class LayoutDisplay : INotifyPropertyChanged
    {
        #region Constructor

        /// <summary>
        /// Set the control during creation
        /// </summary>
        /// <param name="control">The control for the component</param>
        /// <param name="dialogs">Dialogs instance</param>
        /// <param name="customLayoutEventRouter">CustomLayoutEventRouter instance</param>
        public LayoutDisplay(IDisplayComponent control, IDialogs dialogs, ICustomLayoutEventRouter customLayoutEventRouter)
        {
            _dialogs = dialogs;
            _customLayoutEventRouter = customLayoutEventRouter;
            Control = control;
        }

        #endregion

        #region Events

        /// <inheritdoc/>
#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        /// <summary>
        /// Occurs when a control should be removed from the collection
        /// </summary>
        public event EventHandler? RequestRemoval;

        #endregion

        #region Properties

        /// <summary>
        /// Indicate if the controls can be moved
        /// </summary>
        public bool CanMove { get; set; }

        /// <summary>
        /// The control to display
        /// </summary>
        public IDisplayComponent Control { get; }

        /// <summary>
        /// Indicate if the control layout is being edited
        /// </summary>
        public bool LayoutBeingEdited { get; set; }

        /// <summary>
        /// Indicate if the resize handle for the control should be displayed
        /// </summary>
        public bool ShowResizeHandle { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Rename a TabItem
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        public async void RenameItem(object? sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuFlyoutItem { DataContext: LayoutDisplay layoutItem } menuFlyout)
            {
                var newHeader = await _dialogs
                    .ShowTextInputDialogAsync($"Enter new name for \"{layoutItem.Control.Title}\"", layoutItem.Control.Title, menuFlyout.XamlRoot)
                    .ConfigureAwait(true);
                if (!string.IsNullOrEmpty(newHeader))
                {
                    layoutItem.Control.Title = newHeader;
                    await _customLayoutEventRouter.SetLayoutChanged();
                }
            }
        }

        /// <summary>
        /// Request that the control should be removed from the collection
        /// </summary>
        /// <returns></returns>
        public ValueTask RequestRemovalAsync()
        {
            RequestRemoval?.Invoke(this, EventArgs.Empty);

            return ValueTask.CompletedTask;
        }

        #endregion

        #region Variables

        /// <summary>
        /// CustomLayoutEventRouter instance
        /// </summary>
        private readonly ICustomLayoutEventRouter _customLayoutEventRouter;

        /// <summary>
        /// Dialogs instance
        /// </summary>
        private readonly IDialogs _dialogs;

        #endregion
    }
}