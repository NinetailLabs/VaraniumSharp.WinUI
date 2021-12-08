using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.FilterModule;
using VaraniumSharp.WinUI.GroupModule;
using VaraniumSharp.WinUI.SortModule;

namespace VaraniumSharp.WinUI.Interfaces.CustomPaneBase
{
    /// <summary>
    /// Context for CustomLayoutPanes
    /// </summary>
    public interface ICustomPaneContext : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Collection containing the controls to display
        /// </summary>
        ObservableCollection<LayoutDisplay> Components { get; }

        /// <summary>
        /// MenuFlyout that contains the list of controls the user can select from
        /// </summary>
        MenuFlyout ControlMenu { get; }

        /// <summary>
        /// Unique identifier for the layout
        /// </summary>
        Guid LayoutIdentifier { get; set; }

        /// <summary>
        /// Indicate if the user can move the controls
        /// </summary>
        bool MoveControls { get; set; }

        /// <summary>
        /// Indicate if control resizing is enabled
        /// </summary>
        bool ResizeControls { get; set; }

        /// <summary>
        /// Indicate if setting controls should be hidden in the UI
        /// </summary>
        bool ShowControls { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears all the entries in the <see cref="Components"/> collection
        /// </summary>
        /// <returns></returns>
        Task ClearComponentsAsync();

        /// <summary>
        /// Retrieve the collection of filter controls and their currently applied filters
        /// </summary>
        /// <returns>Collection of filter storage models</returns>
        Task<List<FilterStorageModel>> GetControlFiltersAsync();

        /// <summary>
        /// Retrieve collection of sortable controls and their current sort order
        /// </summary>
        /// <returns>Collection of sort storage models</returns>
        Task<List<SortStorageModel>> GetControlSortOrdersAsync();

        /// <summary>
        /// Retrieve the collection of controls to save
        /// </summary>
        Task<List<ControlStorageModel>> GetControlsToSaveAsync();

        /// <summary>
        /// Retrieve collection of group controls and their current groupings
        /// </summary>
        /// <returns>Collection of group storage models</returns>
        Task<List<GroupStorageModel>> GetGroupStorageModelsAsync();

        /// <summary>
        /// Handle the loading of controls
        /// </summary>
        Task HandleControlLoadAsync(List<ControlStorageModel> controls, List<SortStorageModel>? sortOrder, List<GroupStorageModel>? groupOrder, List<FilterStorageModel>? filters);

        /// <summary>
        /// Set the size of the parent container and resize any controls after the current control.
        /// This method should be invoked when the user is dragging the ResizeThumb so only relevant control sizes are adjusted.
        /// </summary>
        /// <param name="content">The IDisplayComponent that is being resized</param>
        Task ResizeControlsWithDragHandleAsync(LayoutDisplay content);

        /// <summary>
        /// Set up all the controls to correctly handle the layout being updated
        /// </summary>
        public Task SetControlDisplayAsync();

        /// <summary>
        /// Set up all the controls to correctly handle moving
        /// </summary>
        Task SetControlMoveAsync();

        /// <summary>
        /// Set up all the controls to correctly handle resizing
        /// </summary>
        Task SetControlResizingAsync();

        /// <summary>
        /// Update the size for the contained controls
        /// </summary>
        /// <param name="height">The height of the parent container</param>
        /// <param name="width">The width of the parent container</param>
        Task UpdateChildrenSizeAsync(double width, double height);

        #endregion
    }
}