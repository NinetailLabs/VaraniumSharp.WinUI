using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using VaraniumSharp.Interfaces.Wrappers;
using VaraniumSharp.Logging;

namespace VaraniumSharp.WinUI.TabViewHelpers
{
    /// <summary>
    /// Assist with persisting the TabView layout
    /// </summary>
    public sealed class TabViewStorageManager
    {
        /// <summary>
        /// DI Constructor
        /// </summary>
        public TabViewStorageManager(IFileWrapper fileWrapper)
        {
            _fileWrapper = fileWrapper;
            _logger = StaticLogger.GetLogger<TabViewStorageManager>();
        }

        /// <summary>
        /// Save the current tab layout to a JSon file
        /// </summary>
        /// <param name="tabs">Collection of tabs to persist</param>
        /// <param name="filePath">Path to JSon file used for persistence</param>
        /// <returns>Indicate if the file was successfully written</returns>
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

        /// <summary>
        /// Load the tab layout from a JSon file
        /// </summary>
        /// <param name="filePath">Path to the JSon to load</param>
        /// <returns></returns>
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
