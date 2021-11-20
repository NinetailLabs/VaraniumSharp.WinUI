using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Threading.Tasks;
using Microsoft.UI.Input;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.HorizontalPane;

namespace VaraniumSharp.WinUI.HorizontalPane
{
    /// <summary>
    /// CustomLayoutPane that lays it's components out horizontally
    /// </summary>
    [AutomaticContainerRegistration(typeof(IHorizontalLayoutPane))]
    [DisplayComponent("Horizontal Layout Pane", ContentIdentifier, "Layout", 100, 100, typeof(IHorizontalLayoutPane))]
    public sealed partial class CustomLayoutPane : IHorizontalLayoutPane, IAsyncDisposable
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public CustomLayoutPane(IHorizontalLayoutPaneContext horizontalLayoutPaneContext)
            : base(horizontalLayoutPaneContext, InputSystemCursorShape.SizeWestEast, Guid.Parse(ContentIdentifier), "Horizontal Pane")
        {
            Context = horizontalLayoutPaneContext;
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// HorizontalLayoutPaneContext instance
        /// </summary>
        public IHorizontalLayoutPaneContext Context { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            if (Context is IAsyncDisposable disposableContext)
            {
                await disposableContext.DisposeAsync();
            }

            await CleanPaneAsync();
        }

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
                if (display.Control.Width + e.HorizontalChange < 5)
                {
                    return;
                }

                display.Control.Width += e.HorizontalChange;
                await Context.ResizeControlsWithDragHandleAsync(display);
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// Control identifier
        /// </summary>
        private const string ContentIdentifier = "add64a44-032b-4f97-a80d-bed7e5dd3fde";

        #endregion
    }
}
