using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VaraniumSharp.WinUI.CustomPaneBase;

namespace VaraniumSharp.WinUI.Interfaces.CustomPaneBase
{
    /// <summary>
    /// Pane that support the customized layout of controls
    /// </summary>
    public interface ICustomLayoutPane : IDisplayComponent
    {
        #region Public Methods

        /// <summary>
        /// Invoke to remove the panes content
        /// </summary>
        Task CleanPaneAsync();

        /// <summary>
        /// Save the layout to disk
        /// </summary>
        Task<List<ControlStorageModel>> GetComponentsForStorageAsync();

        /// <summary>
        /// Get the layout identifier
        /// </summary>
        /// <returns>The control's identifier</returns>
        Guid GetIdentifier();

        /// <summary>
        /// Loads the component details from disk and places them in the collection
        /// </summary>
        /// <param name="contentGuid">Guid of the content that should be loaded</param>
        /// <param name="controls">List of controls that should be loaded</param>
        Task InitAsync(Guid contentGuid, List<ControlStorageModel> controls);

        /// <summary>
        /// Initialize the pane when it doesn't have any controls
        /// </summary>
        /// <param name="contentGuid">Control's identifier</param>
        Task InitAsync(Guid contentGuid);

        #endregion
    }
}