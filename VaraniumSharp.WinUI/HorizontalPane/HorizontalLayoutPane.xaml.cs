using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Core;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.HorizontalPane;
using VaraniumSharp.WinUI.SortModule;

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
            : base(horizontalLayoutPaneContext, CoreCursorType.SizeWestEast)
        {
            Title = "Horizontal Pane";
            Context = horizontalLayoutPaneContext;
            InitializeComponent();
        }

        #endregion

        #region Events

        /// <inheritdoc/>
#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        #endregion

        #region Properties

        /// <inheritdoc/>
        public Guid ContentId => Guid.Parse(ContentIdentifier);

        /// <summary>
        /// HorizontalLayoutPaneContext instance
        /// </summary>
        public IHorizontalLayoutPaneContext Context { get; }

        /// <inheritdoc/>
        public bool ShowResizeHandle { get; set; }

        /// <inheritdoc/>
        public bool StartupLoad { get; set; }

        /// <inheritdoc />
        public string Title { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public Guid UniqueIdentifier { get; set; }

        /// <inheritdoc />
        public Guid InstanceId { get; set; }

        /// <inheritdoc/>
        public async Task CleanPaneAsync()
        {
            await Context.ClearComponentsAsync();
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            if (Context is IAsyncDisposable disposableContext)
            {
                await disposableContext.DisposeAsync();
            }

            await CleanPaneAsync();
        }

        /// <inheritdoc/>
        public async Task<List<ControlStorageModel>> GetComponentsForStorageAsync()
        {
            return await Context.GetControlsToSaveAsync();
        }

        /// <inheritdoc/>
        public Guid GetIdentifier() => Context.LayoutIdentifier;

        /// <inheritdoc/>
        public Task InitAsync()
        {
            // Not used for this control
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task InitAsync(Guid contentGuid)
        {
            Context.LayoutIdentifier = contentGuid;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task InitAsync(Guid contentGuid, List<ControlStorageModel> controls, List<SortStorageModel>? sortOrder)
        {
            Context.LayoutIdentifier = contentGuid;

            await Context.HandleControlLoadAsync(controls, sortOrder);
            await Context.SetControlResizingAsync();
            await Context.UpdateChildrenSizeAsync(Width, Height);
        }

        /// <inheritdoc/>
        public async Task SetControlSizeAsync(double width, double height)
        {
            Width = width;
            Height = height;

            ListControlContainer.Width = width;
            ListControlContainer.Height = height + 12;

            await Context.UpdateChildrenSizeAsync(width, height);
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

        /// <inheritdoc/>
        public async Task<List<SortStorageModel>> GetSortStorageModelsAsync()
        {
            return await Context.GetControlSortOrdersAsync();
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
