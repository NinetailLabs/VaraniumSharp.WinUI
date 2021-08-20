using System.Collections.Generic;
using System.Threading.Tasks;
using VaraniumSharp.WinUI.TabViewHelpers;

namespace VaraniumSharp.WinUI.Interfaces.TabViewHelpers
{
    /// <summary>
    /// Assist with persisting the TabView layout
    /// </summary>
    public interface ITabViewStorageManager
    {
        #region Public Methods

        /// <summary>
        /// Load the tab layout from a JSon file
        /// </summary>
        /// <param name="filePath">Path to the JSon to load</param>
        /// <returns>Collection of TabViewModel entries loaded from the file</returns>
        Task<IEnumerable<TabViewModel>> LoadLayoutAsync(string filePath);

        /// <summary>
        /// Save the current tab layout to a JSon file
        /// </summary>
        /// <param name="tabs">Collection of tabs to persist</param>
        /// <param name="filePath">Path to JSon file used for persistence</param>
        /// <returns>Indicate if the file was successfully written</returns>
        Task SaveLayoutAsync(IEnumerable<TabViewModel> tabs, string filePath);

        #endregion
    }
}
