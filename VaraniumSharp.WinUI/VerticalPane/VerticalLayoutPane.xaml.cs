using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
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
    public sealed partial class VerticalLayoutPane : IVerticalLayoutPane, IAsyncDisposable
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public VerticalLayoutPane(IVerticalLayoutPaneContext verticalLayoutPaneContext)
            : base(verticalLayoutPaneContext, CoreCursorType.SizeNorthSouth)
        {
            Title = "Vertical Pane";
            Context = verticalLayoutPaneContext;
            InitializeComponent();
            Unloaded += OnUnloaded;
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
        /// VerticalLayoutPaneContext instance
        /// </summary>
        public IVerticalLayoutPaneContext Context { get; private set; }

        /// <inheritdoc />
        public bool ShowResizeHandle { get; set; }

        /// <inheritdoc />
        public bool StartupLoad { get; set; }

        /// <inheritdoc />
        public string Title { get; set; }

        /// <inheritdoc />
        public Guid UniqueIdentifier { get; set; }

        #endregion

        #region Public Methods

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
        public async Task InitAsync(Guid contentGuid, List<ControlStorageModel> controls)
        {
            Context.LayoutIdentifier = contentGuid;

            await Context.HandleControlLoadAsync(controls);
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

            ListControlContainer.Width = width;
            ListControlContainer.Height = height + 12;

            await Context.UpdateChildrenSizeAsync(width, height);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Occurs when the control is unloaded
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Bindings.StopTracking();
            Context = null!; // Suppressing this as it will only happen when the control is unloaded
        }

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
