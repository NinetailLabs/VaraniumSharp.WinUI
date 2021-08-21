using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.Dialogs;
using VaraniumSharp.WinUI.Interfaces.HorizontalPane;

namespace VaraniumSharp.WinUI.HorizontalPane
{
    /// <summary>
    /// Context for the HorizontalLayoutPane
    /// </summary>
    [AutomaticContainerRegistration(typeof(IHorizontalLayoutPaneContext))]
    public sealed class HorizontalLayoutPaneContext : CustomPaneContextBase, IHorizontalLayoutPaneContext
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public HorizontalLayoutPaneContext(ICustomLayoutEventRouter customLayoutEventRouter, IControlDiscoveryHelper controlDiscoveryHelper, IDialogs dialogs)
            : base(customLayoutEventRouter, controlDiscoveryHelper, dialogs)
        {
            ShowControls = customLayoutEventRouter.ShowControls;
        }

        #endregion

        #region Private Methods

        /// <inheritdoc />
        protected override async Task HandleControlResizingAsync(LayoutDisplay componentBeingResized, bool resizeOnlyAfterComponent)
        {
            var adjustedIndex = Components.IndexOf(componentBeingResized);
            var childrenCount = Components.Count;
            var sizePerChild = new List<Size>(childrenCount);

            var fixedWidth = Components
                .Where((_, y) => y < adjustedIndex)
                .Sum(x => ((UserControl)x.Control).Width);
            var totalChildrenWidth = Components.Sum(x => ((UserControl)x.Control).Width) - fixedWidth;
            var marginWidth = (childrenCount - 1) * ControlWidthMargin;
            var widthToDivide = Width - marginWidth - fixedWidth;

            for (var c = 0; c < childrenCount; c++)
            {
                var child = (UserControl)Components[c].Control;
                if (c < adjustedIndex)
                {
                    sizePerChild.Add(new Size(child.Width, Height));
                }
                else
                {
                    var percentageWidth = child.Width / totalChildrenWidth * widthToDivide;
                    sizePerChild.Add(new Size(percentageWidth, Height));
                }
            }

            var startIndex = resizeOnlyAfterComponent
                ? adjustedIndex + 1
                : 0;
            await UpdateChildrenSizeAsync(sizePerChild, startIndex).ConfigureAwait(false);
        }

        #endregion

        #region Variables

        /// <summary>
        /// Size of the width margin for the control template
        /// </summary>
        private const int ControlWidthMargin = 8;

        #endregion
    }
}