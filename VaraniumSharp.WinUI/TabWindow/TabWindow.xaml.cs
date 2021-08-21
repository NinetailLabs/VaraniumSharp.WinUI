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
            if (sender is ToggleMenuFlyoutItem flyoutItem)
            {
                _customLayoutEventRouter.SetControlDisplayValue(flyoutItem.IsChecked);
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// CustomLayoutEventRouter instance
        /// </summary>
        private readonly ICustomLayoutEventRouter _customLayoutEventRouter;

        #endregion
    }
}
