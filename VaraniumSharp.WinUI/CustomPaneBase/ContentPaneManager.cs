using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using VaraniumSharp.Attributes;
using VaraniumSharp.Enumerations;
using VaraniumSharp.Interfaces.Wrappers;
using VaraniumSharp.Logging;
using VaraniumSharp.WinUI.CustomShaping;
using VaraniumSharp.WinUI.FilterModule;
using VaraniumSharp.WinUI.GroupModule;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.HorizontalPane;
using VaraniumSharp.WinUI.SortModule;

namespace VaraniumSharp.WinUI.CustomPaneBase
{
    /// <summary>
    /// Assists with the serialization and deserialization of the controls
    /// </summary>
    [AutomaticContainerRegistration(typeof(IContentPaneManager), ServiceReuse.Singleton)]
    public class ContentPaneManager : IContentPaneManager
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public ContentPaneManager(IHorizontalLayoutPane customLayout, ILayoutStorageOptions layoutStorageOptions, 
            IFileWrapper fileWrapper, ICustomLayoutEventRouter customLayoutEventRouter)
        {
            BasePane = customLayout;
            _layoutStorageOptions = layoutStorageOptions;
            _fileWrapper = fileWrapper;
            _customLayoutEventRouter = customLayoutEventRouter;
            _customLayoutEventRouter.SortChanged += CustomLayoutEventRouterSortChanged;
            _customLayoutEventRouter.GroupChanged += CustomLayoutEventRouterOnGroupChanged;
            _customLayoutEventRouter.FilterChanged += CustomLayoutEventRouterOnFilterChanged;
            _customLayoutEventRouter.CustomDataChanged += CustomLayoutEventRouterOnCustomDataChanged;
            _logger = StaticLogger.GetLogger<ContentPaneManager>();
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public IHorizontalLayoutPane BasePane { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public Task DeleteLayoutAsync(Guid layoutId)
        {
            var path = _layoutStorageOptions.GetJsonPath($"{layoutId}.json");
            _fileWrapper.DeleteFile(path);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task SaveLayoutAsync()
        {
            var layoutId = BasePane.GetIdentifier();
            var path = _layoutStorageOptions.GetJsonPath($"{layoutId}.json");
            var wrapper = new LayoutWrapperModel(layoutId, await BasePane.GetComponentsForStorageAsync().ConfigureAwait(false));
            var jsonLayout = JsonSerializer.Serialize(wrapper, LayoutWrapperModelJsonContext.Default.LayoutWrapperModel);
            await _fileWrapper.WriteAllTextAsync(path, jsonLayout);

            await SaveSortOrderAsync().ConfigureAwait(false);
            await SaveGroupOrderAsync().ConfigureAwait(false);
            await SaveFilterOrderAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task ShowSettingPageAsync()
        {
            await _customLayoutEventRouter.SetControlDisplayValue(false);

            await BasePane
                .CleanPaneAsync()
                .ConfigureAwait(false);

            var control = new ControlStorageModel
            {
                ContentId = Guid.Parse("90ef7c67-1cea-4001-aedc-afb8c760a4c8"),
                Title = "Settings",
                Height = 100,
                Width = 100
            };

            await BasePane.InitAsync(control.ContentId, new List<ControlStorageModel>{ control }, null, null, null, null);
        }

        /// <inheritdoc/>
        public async void UpdateBasePaneSize(object sender, SizeChangedEventArgs e)
        {
            var size = e.NewSize;
            await BasePane
                .SetControlSizeAsync(size.Width, size.Height)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task UpdateContentAsync(string tabName)
        {
            await BasePane
                .CleanPaneAsync()
                .ConfigureAwait(true);

            var path = _layoutStorageOptions.GetJsonPath($"{tabName}.json");

            if (_fileWrapper.FileExists(path))
            {
                SortStorageWrapperModel? sortWrapper = null;
                var sortPath = _layoutStorageOptions.GetJsonPath($"{tabName}_sort.json");
                if(_fileWrapper.FileExists(sortPath))
                {
                    var sortJson = await _fileWrapper.ReadAllTextAsync(sortPath).ConfigureAwait(true);
                    sortWrapper = JsonSerializer.Deserialize(sortJson, SortStorageWrapperModelJsonContext.Default.SortStorageWrapperModel);
                }

                GroupStorageWrapperModel? groupWrapper = null;
                var groupPath = _layoutStorageOptions.GetJsonPath($"{tabName}_group.json");
                if (_fileWrapper.FileExists(groupPath))
                {
                    var groupJson = await _fileWrapper.ReadAllTextAsync(groupPath).ConfigureAwait(true);
                    groupWrapper = JsonSerializer.Deserialize(groupJson, GroupStorageWrapperModelJsonContext.Default.GroupStorageWrapperModel);
                }

                FilterStorageWrapperModel? filterWrapper = null;
                var filterPath = _layoutStorageOptions.GetJsonPath($"{tabName}_filter.json");
                if (_fileWrapper.FileExists(filterPath))
                {
                    var filterJson = await _fileWrapper.ReadAllTextAsync(filterPath).ConfigureAwait(true);
                    filterWrapper = JsonSerializer.Deserialize(filterJson, FilterStorageWrapperModelJsonContext.Default.FilterStorageWrapperModel);
                }

                CustomStorageWrapperModel? customWrapper = null;
                var customPath = _layoutStorageOptions.GetJsonPath($"{tabName}_custom.json");
                if (_fileWrapper.FileExists(customPath))
                {
                    var customJson = await _fileWrapper.ReadAllTextAsync(customPath).ConfigureAwait(true);
                    customWrapper = JsonSerializer.Deserialize(customJson,
                        CustomStorageWrapperModelJsonContext.Default.CustomStorageWrapperModel);
                }

                var jsonData = await _fileWrapper.ReadAllTextAsync(path).ConfigureAwait(true);
                var wrapper = JsonSerializer.Deserialize(jsonData, LayoutWrapperModelJsonContext.Default.LayoutWrapperModel);

                if (wrapper == null)
                {
                    _logger.LogWarning("Could not deserialize LayoutWrapperModel {FilePath}", path);
                    return;
                }

                await BasePane
                    .InitAsync(Guid.Parse(tabName), wrapper.Controls, sortWrapper?.ShapingStorage, groupWrapper?.ShapingStorage, filterWrapper?.ShapingStorage, customWrapper?.ShapingStorage)
                    .ConfigureAwait(true);
            }
            else
            {
                await BasePane
                    .InitAsync(Guid.Parse(tabName))
                    .ConfigureAwait(true);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Occurs when the BasePane fires an event to indicate that a control's custom data has changed
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private async void CustomLayoutEventRouterOnCustomDataChanged(object? sender, EventArgs e)
        {
            await SaveCustomDataAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Occurs when the BasePAne fires an event to indicate that a control`s sort order has changed
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private async void CustomLayoutEventRouterOnFilterChanged(object? sender, EventArgs e)
        {
            await SaveFilterOrderAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Occurs when the BasePane fires an event to indicate that a control`s group order has changed
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private async void CustomLayoutEventRouterOnGroupChanged(object? sender, EventArgs e)
        {
            await SaveGroupOrderAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Occurs when the BasePane fires an event to indicate that a control`s sort order has changed
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private async void CustomLayoutEventRouterSortChanged(object? sender, EventArgs e)
        {
            await SaveSortOrderAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Save the custom data for the control pane
        /// </summary>
        private async Task SaveCustomDataAsync()
        {
            try
            {
                await _storageSemaphore.WaitAsync();
                var layoutId = BasePane.GetIdentifier();
                var path = _layoutStorageOptions.GetJsonPath($"{layoutId}_custom.json");
                var wrapper = new CustomStorageWrapperModel(layoutId,
                    await BasePane.GetCustomStorageModelsAsync().ConfigureAwait(false));
                var jsonCustom = JsonSerializer.Serialize(wrapper,
                    CustomStorageWrapperModelJsonContext.Default.CustomStorageWrapperModel);
                await _fileWrapper
                    .WriteAllTextAsync(path, jsonCustom)
                    .ConfigureAwait(false);
            }
            finally
            {
                _storageSemaphore.Release();
            }
        }

        /// <summary>
        /// Save the filter order for the control pane
        /// </summary>
        private async Task SaveFilterOrderAsync()
        {
            try
            {
                await _storageSemaphore.WaitAsync();
                var layoutId = BasePane.GetIdentifier();
                var path = _layoutStorageOptions.GetJsonPath($"{layoutId}_filter.json");
                var wrapper = new FilterStorageWrapperModel(layoutId, await BasePane.GetFilterStorageModelsAsync().ConfigureAwait(false));
                var filterJson = JsonSerializer.Serialize(wrapper, FilterStorageWrapperModelJsonContext.Default.FilterStorageWrapperModel);
                await _fileWrapper
                    .WriteAllTextAsync(path, filterJson)
                    .ConfigureAwait(false);
            }
            finally
            {
                _storageSemaphore.Release();
            }
        }

        /// <summary>
        /// Save the group order for the control pane
        /// </summary>
        private async Task SaveGroupOrderAsync()
        {
            try
            {
                await _storageSemaphore.WaitAsync();
                var layoutId = BasePane.GetIdentifier();
                var path = _layoutStorageOptions.GetJsonPath($"{layoutId}_group.json");
                var wrapper = new GroupStorageWrapperModel(layoutId, await BasePane.GetGroupStorageModelsAsync().ConfigureAwait(false));
                var jsonGroup = JsonSerializer.Serialize(wrapper, GroupStorageWrapperModelJsonContext.Default.GroupStorageWrapperModel);
                await _fileWrapper
                    .WriteAllTextAsync(path, jsonGroup)
                    .ConfigureAwait(false);

            }
            finally
            {
                _storageSemaphore.Release();
            }
        }

        /// <summary>
        /// Save the sort order for the control pane
        /// </summary>
        private async Task SaveSortOrderAsync()
        {
            try
            {
                await _storageSemaphore.WaitAsync();
                var layoutId = BasePane.GetIdentifier();
                var path = _layoutStorageOptions.GetJsonPath($"{layoutId}_sort.json");
                var wrapper = new SortStorageWrapperModel(layoutId, await BasePane.GetSortStorageModelsAsync().ConfigureAwait(false));
                var jsonSort = JsonSerializer.Serialize(wrapper, SortStorageWrapperModelJsonContext.Default.SortStorageWrapperModel);
                await _fileWrapper
                    .WriteAllTextAsync(path, jsonSort)
                    .ConfigureAwait(false);
            }
            finally
            {
                _storageSemaphore.Release();
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// CustomLayoutEventRouter instance
        /// </summary>
        private readonly ICustomLayoutEventRouter _customLayoutEventRouter;

        /// <summary>
        /// FileWrapper instance
        /// </summary>
        private readonly IFileWrapper _fileWrapper;

        /// <summary>
        /// LayoutStorageOptions instance
        /// </summary>
        private readonly ILayoutStorageOptions _layoutStorageOptions;

        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Semaphore used to lock write to the sort storage json file
        /// </summary>
        private readonly SemaphoreSlim _storageSemaphore = new(1);

        #endregion
    }
}