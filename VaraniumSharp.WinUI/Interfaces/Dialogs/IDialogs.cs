using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace VaraniumSharp.WinUI.Interfaces.Dialogs
{
    /// <summary>
    /// Assist with showing Dialog options to the user
    /// </summary>
    public interface IDialogs
    {
        #region Public Methods

        /// <summary>
        /// Show a confirmation dialog box
        /// </summary>
        /// <param name="title">The title of the dialog</param>
        /// <param name="message">The prompt for the user</param>
        /// <param name="root">XamlRoot of the control the dialog is tied to</param>
        /// <returns>True if user clicked "Yes" button, otherwise false</returns>
        Task<bool> ShowConfirmationDialog(string title, string message, XamlRoot root);

        /// <summary>
        /// Show a dialog box that displays a message to the user
        /// </summary>
        /// <param name="title">The title of the dialog box</param>
        /// <param name="message">The message to display to the user</param>
        /// <param name="root">XamlRoot of the control the dialog is tied to</param>
        Task ShowMessageDialogAsync(string title, string message, XamlRoot root);

        /// <summary>
        /// Show a dialog box that takes text input
        /// </summary>
        /// <param name="title">Title of the dialog box</param>
        /// <param name="currentValue">The current value of the text</param>
        /// <param name="root">XamlRoot of the control the dialog is tied to</param>
        /// <returns>New header or <see cref="string.Empty"/> if the user cancelled out of the dialog</returns>
        Task<string> ShowTextInputDialogAsync(string title, string currentValue, XamlRoot root);

        #endregion
    }
}
