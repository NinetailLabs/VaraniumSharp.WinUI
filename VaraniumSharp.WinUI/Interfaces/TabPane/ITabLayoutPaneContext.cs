using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;

namespace VaraniumSharp.WinUI.Interfaces.TabPane
{
    /// <summary>
    /// Context for the TabLayoutPane
    /// </summary>
    public interface ITabLayoutPaneContext : ICustomPaneContext
    {
        #region Public Methods

        /// <summary>
        /// Occurs when the user closes a TabView
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="args">Event arguments</param>
        void OnTabClosedAsync(TabView? sender, TabViewTabCloseRequestedEventArgs args);

        #endregion
    }
}