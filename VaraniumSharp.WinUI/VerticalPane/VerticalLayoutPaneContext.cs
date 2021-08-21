using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.Dialogs;
using VaraniumSharp.WinUI.Interfaces.VerticalPane;

namespace VaraniumSharp.WinUI.VerticalPane
{
    /// <summary>
    /// Context for the VerticalLayoutPane
    /// </summary>
    [AutomaticContainerRegistration(typeof(IVerticalLayoutPaneContext))]
    public class VerticalLayoutPaneContext : CustomPaneContextBase, IVerticalLayoutPaneContext
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public VerticalLayoutPaneContext(ICustomLayoutEventRouter customLayoutEventRouter, IControlDiscoveryHelper controlDiscoveryHelper, IDialogs dialogs)
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

            var fixedHeight = Components
                .Where((_, y) => y < adjustedIndex)
                .Sum(x => ((UserControl)x.Control).Height);
            var totalChildrenHeight = Components.Sum(x => ((UserControl)x.Control).Height) - fixedHeight;
            var marginHeight = (childrenCount - 1) * ControlHeightMargin;
            var heightToDivide = Height - marginHeight - fixedHeight;

            for (var h = 0; h < childrenCount; h++)
            {
                var child = (UserControl)Components[h].Control;
                if (h < adjustedIndex)
                {
                    sizePerChild.Add(new Size(Width, child.Height));
                }
                else
                {
                    var percentageHeight = child.Height / totalChildrenHeight * heightToDivide;
                    sizePerChild.Add(new Size(Width, percentageHeight));
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
        private const int ControlHeightMargin = 8;

        #endregion
    }
}