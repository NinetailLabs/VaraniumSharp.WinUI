using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using VaraniumSharp.Attributes;
using VaraniumSharp.Logging;
using VaraniumSharp.WinUI.Interfaces.Dialogs;

namespace VaraniumSharp.WinUI.Dialog
{
    /// <summary>
    /// Assist with showing Dialog options to the user
    /// </summary>
    [AutomaticContainerRegistration(typeof(IDialogs), Enumerations.ServiceReuse.Singleton)]
    public class Dialogs : IDialogs
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public Dialogs()
        {
            _logger = StaticLogger.GetLogger<Dialogs>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Show a confirmation dialog box
        /// </summary>
        /// <param name="title">The title of the dialog</param>
        /// <param name="message">The prompt for the user</param>
        /// <param name="root"></param>
        /// <returns>True if user clicked "Yes" button, otherwise false</returns>
        public async Task<bool> ShowConfirmationDialog(string title, string message, XamlRoot root)
        {
            var textBlock = new TextBlock
            {
                Text = message,
                VerticalAlignment = VerticalAlignment.Bottom
            };

            var dialog = new ContentDialog
            {
                Content = textBlock,
                Title = title,
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = root
            };

            try
            {
                return await dialog.ShowAsync() == ContentDialogResult.Primary;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while showing the confirmation dialog");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task ShowMessageDialogAsync(string title, string message, XamlRoot root)
        {
            var textBlock = new TextBlock
            {
                Text = message,
                VerticalAlignment = VerticalAlignment.Bottom
            };

            var dialog = new ContentDialog
            {
                Content = textBlock,
                Title = title,
                IsSecondaryButtonEnabled = false,
                PrimaryButtonText = "Ok",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = root
            };

            try
            {
                await dialog.ShowAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while showing the MessageDialog");
            }
        }

        /// <inheritdoc/>
        public async Task<string> ShowTextInputDialogAsync(string title, string currentValue, XamlRoot root)
        {
            var inputTextBox = new TextBox
            {
                AcceptsReturn = false,
                Text = currentValue,
                VerticalAlignment = VerticalAlignment.Bottom
            };

            var dialog = new ContentDialog
            {
                Content = inputTextBox,
                Title = title,
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = "Ok",
                SecondaryButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = root
            };

            try
            {
                return await dialog.ShowAsync() == ContentDialogResult.Primary
                    ? inputTextBox.Text
                    : string.Empty;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while showing the TextInputDialog");
                return string.Empty;
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly ILogger _logger;

        #endregion
    }
}
