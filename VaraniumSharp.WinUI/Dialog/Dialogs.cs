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
    public sealed class Dialogs : IDialogs
    {
        /// <summary>
        /// DI Constructor
        /// </summary>
        public Dialogs()
        {
            _logger = StaticLogger.GetLogger<Dialogs>();
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
                _logger.LogError(exception, "An error occured while showing the TextInputDialog");
                return string.Empty;
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
                _logger.LogError(exception, "An error occured while showing the MessageDialog");
            }
        }

        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly ILogger _logger;
    }
}
