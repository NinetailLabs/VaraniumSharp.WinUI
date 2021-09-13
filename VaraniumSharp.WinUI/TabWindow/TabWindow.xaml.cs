using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.Attributes;
using VaraniumSharp.Enumerations;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.TabWindow;

namespace VaraniumSharp.WinUI.TabWindow
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    [AutomaticContainerRegistration(typeof(TabWindow), ServiceReuse.Singleton)]
    public sealed partial class TabWindow
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public TabWindow(ITabWindowContext tabWindowContext, ICustomLayoutEventRouter customLayoutEventRouter)
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            Context = tabWindowContext;
            TabViewer.Loaded += TabViewer_Loaded;
            LayoutPane.Children.Add(tabWindowContext.ContentPaneManager.BasePane as UIElement);
            _customLayoutEventRouter = customLayoutEventRouter;
            _customLayoutEventRouter.ControlDisplayChanged += CustomLayoutEventRouterOnControlDisplayChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        /// TabViewerContext instance
        /// </summary>
        public ITabWindowContext Context { get; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Occurs when a request is made to show/hide controls
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="showControls">Indicate if controls should be shown or hidden</param>
        private void CustomLayoutEventRouterOnControlDisplayChanged(object? sender, bool showControls)
        {
            ControlShowItem.Text = showControls
                ? HideText
                : ShowText;
        }

        /// <summary>
        /// Occurs when the TabViewer has loaded
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void TabViewer_Loaded(object sender, RoutedEventArgs e)
        {
            Context.SetXamlRoot(TabViewer.XamlRoot);
        }

        /// <summary>
        /// Occurs when the "Show layout control" flyout item is toggled
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void ToggleMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem flyoutItem)
            {
                var showControls = flyoutItem.Text == ShowText;
                _customLayoutEventRouter.SetControlDisplayValue(showControls);
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// Text to display when controls can be shown
        /// </summary>
        private const string ShowText = "Show Layout Controls";

        /// <summary>
        /// Text to display when controls can be hidden
        /// </summary>
        private const string HideText = "Hide Layout Controls";

        /// <summary>
        /// CustomLayoutEventRouter instance
        /// </summary>
        private readonly ICustomLayoutEventRouter _customLayoutEventRouter;

        #endregion
    }
}
