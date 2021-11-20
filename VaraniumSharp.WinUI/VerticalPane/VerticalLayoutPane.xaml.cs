using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Threading.Tasks;
using Microsoft.UI.Input;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.VerticalPane;

namespace VaraniumSharp.WinUI.VerticalPane
{
    /// <summary>
    /// CustomLayoutPane that lays out its components vertically
    /// </summary>
    [AutomaticContainerRegistration(typeof(IVerticalLayoutPane))]
    [DisplayComponent("Vertical Layout Pane", ContentIdentifier, "Layout", 100, 100, typeof(IVerticalLayoutPane))]
    public sealed partial class VerticalLayoutPane : IVerticalLayoutPane
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public VerticalLayoutPane(IVerticalLayoutPaneContext verticalLayoutPaneContext)
            : base(verticalLayoutPaneContext, InputSystemCursorShape.SizeNorthSouth, Guid.Parse(ContentIdentifier), "Vertical Pane")
        {
            Context = verticalLayoutPaneContext;
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// VerticalLayoutPaneContext instance
        /// </summary>
        public IVerticalLayoutPaneContext Context { get; }

        #endregion

        #region Public Methods

       

        /// <inheritdoc />
        public override Task SetControlSizeAsync(double width, double height)
        {
            ListControlContainer.Width = width;
            ListControlContainer.Height = height + 12;

            return base.SetControlSizeAsync(width, height);
        }

        #endregion

        #region Private Methods

        /// <inheritdoc/>
        protected override async void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is Thumb { DataContext: LayoutDisplay display })
            {
                if (display.Control.Height + e.VerticalChange < 5)
                {
                    return;
                }

                display.Control.Height += e.VerticalChange;
                await Context.ResizeControlsWithDragHandleAsync(display);
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// Control identifier
        /// </summary>
        private const string ContentIdentifier = "583714ef-e3ff-4919-bcfd-63dd2b15b8b9";

        #endregion
    }
}
