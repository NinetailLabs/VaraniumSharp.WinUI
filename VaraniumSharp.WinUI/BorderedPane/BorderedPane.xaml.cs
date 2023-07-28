using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Threading.Tasks;
using Microsoft.UI.Input;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.BorderedPane;

namespace VaraniumSharp.WinUI.BorderedPane
{
    /// <summary>
    /// CustomLayoutPane that lays out its components vertically with a title bordered wrapping the content
    /// </summary>
    [AutomaticContainerRegistration(typeof(IBorderedPane))]
    [DisplayComponent("Bordered Pane", ContentIdentifier, "Layout", 100, 100, typeof(IBorderedPane))]
    public sealed partial class BorderedPane : IBorderedPane
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public BorderedPane(IBorderedPaneContext borderedPaneContext)
            : base(borderedPaneContext, InputSystemCursorShape.SizeNorthSouth, Guid.Parse(ContentIdentifier), "Bordered Pane")
        {
            Context = borderedPaneContext;
            Context.SetTitleFunctions(() => Title, newTitle => Title = newTitle);
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// VerticalLayoutPaneContext instance
        /// </summary>
        public IBorderedPaneContext Context { get; }

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
        private const string ContentIdentifier = "ed934897-7b57-473c-a9a1-c8c29001598e";

        #endregion
    }
}
