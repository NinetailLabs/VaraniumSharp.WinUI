using Microsoft.UI.Xaml;

namespace VaraniumSharp.WinUI.Interfaces.Pickers
{
    /// <summary>
    /// Assist with setting the main <see cref="Window"/> that Pickers will be associated with.
    /// When using the <see cref="WinUI.TabWindow.TabWindow"/> this class will be set up automatically.
    /// </summary>
    public interface IOwnerWindow
    {
        #region Properties

        /// <summary>
        /// The main parent Window that the Pickers should be assigned to
        /// </summary>
        Window ParentWindow { get; set; }

        #endregion
    }
}