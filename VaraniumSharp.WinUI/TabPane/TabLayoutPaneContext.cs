using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.Dialogs;
using VaraniumSharp.WinUI.Interfaces.TabPane;

namespace VaraniumSharp.WinUI.TabPane
{
    /// <summary>
    /// Context for the TabLayoutPane
    /// </summary>
    [AutomaticContainerRegistration(typeof(ITabLayoutPaneContext))]
    public class TabLayoutPaneContext : CustomPaneContextBase, ITabLayoutPaneContext
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public TabLayoutPaneContext(ICustomLayoutEventRouter customLayoutEventRouter,
            IControlDiscoveryHelper controlDiscoveryHelper, IDialogs dialogs)
            : base(customLayoutEventRouter, controlDiscoveryHelper, dialogs)
        {
            ShowControls = customLayoutEventRouter.ShowControls;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public async void OnTabClosedAsync(TabView? sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (args.Item is LayoutDisplay layoutToRemove)
            {
                Components.Remove(layoutToRemove);
                await CustomLayoutEventRouter
                    .SetLayoutChanged()
                    .ConfigureAwait(false);
            }
        }

        #endregion

        #region Private Methods

        /// <inheritdoc />
        protected override async Task HandleControlResizingAsync(LayoutDisplay componentBeingResized, bool resizeOnlyAfterComponent)
        {
            var childrenCount = Components.Count;
            var sizePerChild = new List<Size>(childrenCount);
            for (var c = 0; c < childrenCount; c++)
            {
                sizePerChild.Add(new Size(Width, Height));
            }

            await UpdateChildrenSizeAsync(sizePerChild, 0).ConfigureAwait(false);
        }

        #endregion
    }
}