using System;
using System.Threading.Tasks;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.TabPane;

namespace VaraniumSharp.WinUI.TabPane
{
    /// <summary>
    /// CustomLayoutPane that lays out its components as tabs
    /// </summary>
    [AutomaticContainerRegistration(typeof(ITabLayoutPane))]
    [DisplayComponent("Tab Layout Pane", ContentIdentifier, "Layout", 100, 100, typeof(ITabLayoutPane))]
    public sealed partial class TabLayoutLayoutPane : ITabLayoutPane, IAsyncDisposable
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public TabLayoutLayoutPane(ITabLayoutPaneContext tabLayoutPaneContext)
            : base(Guid.Parse(ContentIdentifier), tabLayoutPaneContext, "Tab Pane")
        {
            Context = tabLayoutPaneContext;
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// TabLayoutPaneContext instance
        /// </summary>
        public ITabLayoutPaneContext Context { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
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
            TabContainer.Width = width;
            TabContainer.Height = height + 12;

            return base.SetControlSizeAsync(width, height);
        }

        #endregion

        #region Variables

        /// <summary>
        /// Control identifier
        /// </summary>
        private const string ContentIdentifier = "89fcdda9-018c-4652-865a-89d4d2e5f68a";

        #endregion
    }
}
