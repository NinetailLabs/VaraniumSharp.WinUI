using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.GroupModule;
using VaraniumSharp.WinUI.SortModule;

namespace VaraniumSharp.WinUI.Interfaces.CustomPaneBase
{
    /// <summary>
    /// Pane that support the customized layout of controls
    /// </summary>
    public interface ICustomLayoutPane : IDisplayComponent
    {
        #region Properties

        /// <summary>
        /// Unique identifier used during control initialization
        /// </summary>
        public Guid UniqueIdentifier { get; set; }

        #endregion

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
        /// Retrieve group storage models for sub-components
        /// </summary>
        /// <returns>Collection of group storage models for sub components</returns>
        Task<List<GroupStorageModel>> GetGroupStorageModelsAsync();

        /// <summary>
        /// Get the layout identifier
        /// </summary>
        /// <returns>The control's identifier</returns>
        Guid GetIdentifier();

        /// <summary>
        /// Retrieve sort storage models for sub-components
        /// </summary>
        /// <returns>Collection of sort storage models for sub components</returns>
        Task<List<SortStorageModel>> GetSortStorageModelsAsync();

        /// <summary>
        /// Loads the component details from disk and places them in the collection
        /// </summary>
        /// <param name="contentGuid">Guid of the content that should be loaded</param>
        /// <param name="controls">List of controls that should be loaded</param>
        /// <param name="sortOrder">The default order in which the control content should be sorted</param>
        /// <param name="groupOrder">The default order in which the control content should be grouped</param>
        Task InitAsync(Guid contentGuid, List<ControlStorageModel> controls, List<SortStorageModel>? sortOrder, List<GroupStorageModel>? groupOrder);

        /// <summary>
        /// Initialize the pane when it doesn't have any controls
        /// </summary>
        /// <param name="contentGuid">Control's identifier</param>
        Task InitAsync(Guid contentGuid);

        #endregion
    }
}