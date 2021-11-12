using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;
using VaraniumSharp.WinUI.TabViewHelpers;

namespace VaraniumSharp.WinUI.Interfaces.TabWindow
{
    /// <summary>
    /// Context for TabView window
    /// </summary>
    public interface ITabWindowContext : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// ContentPaneManager instance
        /// </summary>
        IContentPaneManager ContentPaneManager { get; }

        /// <summary>
        /// Indicate if context menu items should be enabled or not
        /// </summary>
        bool EnableContextMenuItems { get; }

        /// <summary>
        /// TabViewKeyboardAccelerators instance
        /// </summary>
        TabViewKeyboardAccelerators KeyboardAccelerators { get; set; }

        /// <summary>
        /// The maximum size that a tab is allowed to be
        /// </summary>
        double MaxTabViewSize { get; set; }

        /// <summary>
        /// The selected index in the Tab view
        /// </summary>
        int SelectedIndex { get; set; }

        /// <summary>
        /// Collection of all tabs
        /// </summary>
        ObservableCollection<TabViewItem> Tabs { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Occurs when the TabView Add button is clicked
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="args">Event arguments</param>
        void OnAddClickedAsync(TabView? sender, object args);

        /// <summary>
        /// Occurs when the TabView is loaded
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        void OnLoadedAsync(object sender, RoutedEventArgs e);

        /// <summary>
        /// Occurs when the TabSelection changes
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event argument</param>
        void OnSelectionChangedAsync(object? sender, SelectionChangedEventArgs e);

        /// <summary>
        /// Occurs when the user closes a TabView
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="args">Event arguments</param>
        void OnTabClosedAsync(TabView? sender, TabViewTabCloseRequestedEventArgs args);

        /// <summary>
        /// Request saving of the layout
        /// </summary>
        Task SaveLayoutAsync();

        /// <summary>
        /// Set the XamlRoot for the containing window
        /// </summary>
        /// <param name="root">XamlRoot for the container</param>
        void SetXamlRoot(XamlRoot root);

        /// <summary>
        /// Request the display of the setting pane
        /// </summary>
        /// <returns></returns>
        Task ShowSettingPaneAsync();

        /// <summary>
        /// Occurs when the size of the window changes
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        void WindowSizeChanged(object sender, SizeChangedEventArgs e);

        #endregion
    }
}