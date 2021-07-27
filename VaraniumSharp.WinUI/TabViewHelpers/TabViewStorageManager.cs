using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using VaraniumSharp.Attributes;
using VaraniumSharp.Interfaces.Wrappers;
using VaraniumSharp.Logging;
using VaraniumSharp.WinUI.Interfaces.TabViewHelpers;

namespace VaraniumSharp.WinUI.TabViewHelpers
{
    /// <summary>
    /// Assist with persisting the TabView layout
    /// </summary>
    [AutomaticContainerRegistration(typeof(ITabViewStorageManager))]
    public sealed class TabViewStorageManager : ITabViewStorageManager
    {
        /// <summary>
        /// DI Constructor
        /// </summary>
        public TabViewStorageManager(IFileWrapper fileWrapper)
        {
            _fileWrapper = fileWrapper;
            _logger = StaticLogger.GetLogger<TabViewStorageManager>();
        }

        /// <inheritdoc/>
        public Task SaveLayoutAsync(IEnumerable<TabViewModel> tabs, string filePath)
        {
            try
            {
                var jsonTabs = JsonSerializer.Serialize(new TabsContainerModel(tabs), TabsContainerJsonContext.Default.TabsContainerModel);
                _fileWrapper
                    .WriteAllText(filePath, jsonTabs);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occured while attempting to load Tabs from {FilePath}", filePath);
                throw;
            }
        }

        /// <inheritdoc/>
        public Task<IEnumerable<TabViewModel>> LoadLayoutAsync(string filePath)
        {
            try
            {
                var jsonData = _fileWrapper
                    .ReadAllText(filePath);
                var tabsContainer = JsonSerializer.Deserialize<TabsContainerModel>(jsonData, TabsContainerJsonContext.Default.TabsContainerModel);
                return Task.FromResult(tabsContainer?.Tabs ?? Enumerable.Empty<TabViewModel>());
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occured while attempting to load Tabs from {FilePath}", filePath);
                throw;
            }
        }

        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// FileWrapper instance
        /// </summary>
        private readonly IFileWrapper _fileWrapper;
    }
}
