using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using ABI.Microsoft.UI.Xaml.Shapes;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using VaraniumSharp.Attributes;
using VaraniumSharp.Enumerations;
using VaraniumSharp.Interfaces.Wrappers;
using VaraniumSharp.Logging;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.HorizontalPane;
using Path = System.IO.Path;

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

            await BasePane.InitAsync(control.ContentId, new List<ControlStorageModel>{ control });
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
                .ConfigureAwait(false);

            var path = _layoutStorageOptions.GetJsonPath($"{tabName}.json");

            if (_fileWrapper.FileExists(path))
            {
                var jsonData = await _fileWrapper.ReadAllTextAsync(path);
                var wrapper = JsonSerializer.Deserialize<LayoutWrapperModel>(jsonData, LayoutWrapperModelJsonContext.Default.LayoutWrapperModel);

                if (wrapper == null)
                {
                    _logger.LogWarning("Could not deserialize LayoutWrapperModel {FilePath}", path);
                    return;
                }

                await BasePane
                    .InitAsync(Guid.Parse(tabName), wrapper.Controls)
                    .ConfigureAwait(false);
            }
            else
            {
                await BasePane
                    .InitAsync(Guid.Parse(tabName))
                    .ConfigureAwait(false);
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

        #endregion
    }
}