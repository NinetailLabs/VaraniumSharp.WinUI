using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.WinUI.GroupModule;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;
using VaraniumSharp.WinUI.SortModule;

namespace VaraniumSharp.WinUI.CustomPaneBase
{
    /// <summary>
    /// Base class for use with custom layout pane UI
    /// </summary>
    public class LayoutPaneBase : UserControl, ICustomLayoutPane
    {
        /// <summary>
        /// Construct and set required properties
        /// </summary>
        /// <param name="contentId">Control's content Id</param>
        /// <param name="context">Context for the control</param>
        /// <param name="title">Control's title</param>
        public LayoutPaneBase(Guid contentId, ICustomPaneContext context, string title)
        {
            ContentId = contentId;
            GenericContext = context;
            Title = title;
        }

        #region Events

        /// <inheritdoc />
#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        #endregion

        #region Properties

        /// <inheritdoc />
        public Guid ContentId { get; protected init; }

        /// <summary>
        /// Context for the control
        /// </summary>
        public ICustomPaneContext GenericContext { get; protected init; }

        /// <inheritdoc />
        public Guid InstanceId { get; set; }

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
            await GenericContext.ClearComponentsAsync();
        }

        /// <inheritdoc />
        public async Task<List<ControlStorageModel>> GetComponentsForStorageAsync()
        {
            return await GenericContext.GetControlsToSaveAsync();
        }

        /// <inheritdoc />
        public async Task<List<GroupStorageModel>> GetGroupStorageModelsAsync()
        {
            return await GenericContext.GetGroupStorageModelsAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Guid GetIdentifier() => GenericContext.LayoutIdentifier;

        /// <inheritdoc />
        public async Task<List<SortStorageModel>> GetSortStorageModelsAsync()
        {
            return await GenericContext.GetControlSortOrdersAsync();
        }

        /// <inheritdoc />
        public virtual async Task InitAsync(Guid contentGuid, List<ControlStorageModel> controls, List<SortStorageModel>? sortOrder, List<GroupStorageModel>? groupOrder)
        {
            GenericContext.LayoutIdentifier = contentGuid;

            await GenericContext.HandleControlLoadAsync(controls, sortOrder, groupOrder);
            await GenericContext.SetControlResizingAsync();
            await GenericContext.UpdateChildrenSizeAsync(Width, Height);
        }

        /// <inheritdoc />
        public virtual Task InitAsync(Guid contentGuid)
        {
            GenericContext.LayoutIdentifier = contentGuid;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual async Task SetControlSizeAsync(double width, double height)
        {
            Width = width;
            Height = height;
            
            await GenericContext.UpdateChildrenSizeAsync(width, height);
        }

        /// <inheritdoc />
        public virtual Task InitAsync()
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}