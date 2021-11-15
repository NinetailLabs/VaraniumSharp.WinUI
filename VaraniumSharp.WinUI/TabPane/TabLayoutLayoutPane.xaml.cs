using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.TabPane;
using VaraniumSharp.WinUI.SortModule;

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
        {
            Title = "Tab Pane";
            Context = tabLayoutPaneContext;
            InitializeComponent();
        }

        #endregion

        #region Events

        /// <inheritdoc />
#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        #endregion

        #region Properties

        /// <inheritdoc />
        public Guid ContentId => Guid.Parse(ContentIdentifier);

        /// <summary>
        /// TabLayoutPaneContext instance
        /// </summary>
        public ITabLayoutPaneContext Context { get; }

        /// <inheritdoc />
        public bool ShowResizeHandle { get; set; }

        /// <inheritdoc />
        public bool StartupLoad { get; set; }

        /// <inheritdoc />
        public string Title { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public Guid UniqueIdentifier { get; set; }

        /// <inheritdoc />
        public Guid InstanceId { get; set; }

        /// <inheritdoc />
        public async Task CleanPaneAsync()
        {
            await Context.ClearComponentsAsync();
        }

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
        public async Task<List<ControlStorageModel>> GetComponentsForStorageAsync()
        {
            return await Context.GetControlsToSaveAsync();
        }

        /// <inheritdoc />
        public Guid GetIdentifier() => Context.LayoutIdentifier;

        /// <inheritdoc />
        public Task InitAsync()
        {
            // Not used for this control
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task InitAsync(Guid contentGuid, List<ControlStorageModel> controls, List<SortStorageModel>? sortOrder)
        {
            Context.LayoutIdentifier = contentGuid;

            await Context.HandleControlLoadAsync(controls, sortOrder);
            await Context.SetControlResizingAsync();
            await Context.UpdateChildrenSizeAsync(Width, Height);
        }

        /// <inheritdoc />
        public Task InitAsync(Guid contentGuid)
        {
            Context.LayoutIdentifier = contentGuid;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task SetControlSizeAsync(double width, double height)
        {
            Width = width;
            Height = height;

            TabContainer.Width = width;
            TabContainer.Height = height + 12;

            await Context.UpdateChildrenSizeAsync(width, height);
        }

        /// <inheritdoc />
        public async Task<List<SortStorageModel>> GetSortStorageModelsAsync()
        {
            return await Context.GetControlSortOrdersAsync();
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
