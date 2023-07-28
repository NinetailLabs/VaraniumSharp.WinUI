using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.Logging;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.Dialogs;
using VaraniumSharp.Extensions;
using VaraniumSharp.WinUI.CustomShaping;
using VaraniumSharp.WinUI.FilterModule;
using VaraniumSharp.WinUI.GroupModule;
using VaraniumSharp.WinUI.Interfaces.CustomShaping;
using VaraniumSharp.WinUI.SortModule;

namespace VaraniumSharp.WinUI.CustomPaneBase
{
    /// <summary>
    ///     Base class for CustomPane context classes
    /// </summary>
    public abstract class CustomPaneContextBase : ICustomPaneContext, IAsyncDisposable
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customLayoutEventRouter">CustomLayoutEventRouter used to listen for layout change events</param>
        /// <param name="controlDiscoveryHelper">Helper used to discover controls</param>
        /// <param name="dialogs">Dialogs instance</param>
        protected CustomPaneContextBase(ICustomLayoutEventRouter customLayoutEventRouter,
            IControlDiscoveryHelper controlDiscoveryHelper, IDialogs dialogs)
        {
            CustomLayoutEventRouter = customLayoutEventRouter;
            CustomLayoutEventRouter.ControlDisplayChanged += _customLayoutEventRouter_ControlDisplayChanged;
            _controlDiscoveryHelper = controlDiscoveryHelper;
            Dialogs = dialogs;

            Components = new ObservableCollection<LayoutDisplay>();
            Components.CollectionChanged += Components_CollectionChanged;
            Logger = StaticLogger.GetLogger<CustomPaneContextBase>();
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
        public ObservableCollection<LayoutDisplay> Components { get; }

        /// <inheritdoc />
        public MenuFlyout ControlMenu
        {
            get
            {
                if (_menuFlyout != null)
                {
                    return _menuFlyout;
                }

                _menuFlyout = _controlDiscoveryHelper.GetMenuItemWithAvailableControls(MenuButtonClick);
                return _menuFlyout;
            }
        }

        /// <inheritdoc />
        public Guid LayoutIdentifier { get; set; }

        /// <inheritdoc />
        public bool MoveControls
        {
            get => _moveControls;
            set
            {
                _moveControls = value;
                if (ResizeControls)
                {
                    _resizeControls = false;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResizeControls)));
                    SetControlResizingAsync();
                }

                SetControlMoveAsync();
            }
        }

        /// <inheritdoc />
        public bool ResizeControls
        {
            get => _resizeControls;
            set
            {
                _resizeControls = value;
                if (MoveControls)
                {
                    _moveControls = false;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MoveControls)));
                    SetControlMoveAsync();
                }

                SetControlResizingAsync();
            }
        }

        /// <inheritdoc />
        public bool ShowControls
        {
            get => _showControls;
            set
            {
                _showControls = value;
                SetControlDisplayAsync();
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public async Task ClearComponentsAsync()
        {
            foreach (var control in Components.Select(x => x.Control))
            {
                if(control is ISortableDisplayComponent sortableDisplayComponent)
                {
                    sortableDisplayComponent.SortChanged -= SortableDisplayComponent_SortChanged;
                }

                if (control is IGroupingDisplayComponent groupingDisplayComponent)
                {
                    groupingDisplayComponent.GroupChanged -= GroupingDisplayComponentOnGroupChanged;
                }

                if (control is IFilteringDisplayComponent filteringDisplayComponent)
                {
                    filteringDisplayComponent.FilterChanged -= FilteringDisplayComponentOnFilterChanged;
                }
                
                if (control is IAsyncDisposable disposableControl)
                {
                    Logger.LogDebug("Disposing {ContentId}", control.ContentId);
                    await disposableControl
                        .DisposeAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    Logger.LogDebug("Not disposing {ContentId} as it does not implement IAsyncDisposable", control.ContentId);
                }
            }
            
            Components.Clear(displays =>
            {
                foreach (var layoutDisplay in displays)
                {
                    layoutDisplay.RequestRemoval -= Entry_RequestRemoval;
                }
            });
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            CustomLayoutEventRouter.ControlDisplayChanged -= _customLayoutEventRouter_ControlDisplayChanged;
            Components.CollectionChanged -= Components_CollectionChanged;

            await ClearComponentsAsync();
        }

        /// <inheritdoc />
        public async Task<List<CustomStorageModel>> GetControlCustomDataAsync()
        {
            var resultList = new List<CustomStorageModel>();

            foreach (var component in Components.Select(x => x.Control))
            {
                if (component is ICustomLayoutPane customPane)
                {
                    var storageEntry = new CustomStorageModel
                    {
                        InstanceId = customPane.InstanceId
                    };
                    storageEntry.SubEntries.AddRange(await customPane.GetCustomStorageModelsAsync());
                    resultList.Add(storageEntry);
                }
                else
                {
                    if (component is ICustomShapingDisplayComponent sortableDisplayComponent)
                    {
                        resultList.Add(new()
                        {
                            InstanceId = sortableDisplayComponent.InstanceId,
                            ShapingEntries = sortableDisplayComponent.EntriesShapedBy.Select(x => new CustomEntryStorageModel((CustomShapingEntry)x)).ToList()
                        });
                    }
                }
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task<List<FilterStorageModel>> GetControlFiltersAsync()
        {
            var resultList = new List<FilterStorageModel>();

            foreach (var component in Components.Select(x => x.Control))
            {
                if (component is ICustomLayoutPane customPane)
                {
                    var storageEntry = new FilterStorageModel
                    {
                        InstanceId = customPane.InstanceId
                    };
                    storageEntry.SubEntries.AddRange(await customPane.GetFilterStorageModelsAsync().ConfigureAwait(false));
                }
                else
                {
                    if (component is IFilteringDisplayComponent filteringDisplayComponent)
                    {
                        resultList.Add(new()
                        {
                            InstanceId = filteringDisplayComponent.InstanceId,
                            ShapingEntries = filteringDisplayComponent.FilterablePropertyModule.FilterControls.Select(x => new FilterEntryStorageModel(((IFilterControl)x).ShapingEntry)).ToList()
                        });
                    }
                }
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task<List<SortStorageModel>> GetControlSortOrdersAsync()
        {
            var resultList = new List<SortStorageModel>();

            foreach(var component in Components.Select(x => x.Control))
            {
                if(component is ICustomLayoutPane customPane)
                {
                    var storageEntry = new SortStorageModel
                    {
                        InstanceId = customPane.InstanceId
                    };
                    storageEntry.SubEntries.AddRange(await customPane.GetSortStorageModelsAsync());
                    resultList.Add(storageEntry);
                }
                else
                {
                    if(component is ISortableDisplayComponent sortableDisplayComponent)
                    {
                        resultList.Add(new()
                        {
                            InstanceId = sortableDisplayComponent.InstanceId,
                            ShapingEntries = sortableDisplayComponent.SortablePropertyModule.EntriesShapedBy.Select(x => new SortEntryStorageModel((SortableShapingEntry)x)).ToList()
                        });
                    }
                }
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task<List<ControlStorageModel>> GetControlsToSaveAsync()
        {
            var resultList = new List<ControlStorageModel>();

            foreach (var component in Components)
            {
                if (component.Control is ICustomLayoutPane customPane)
                {
                    var control = new ControlStorageModel(component.Control);
                    control.ChildItems.AddRange(await customPane.GetComponentsForStorageAsync().ConfigureAwait(false));
                    resultList.Add(control);
                }
                else
                {
                    resultList.Add(new ControlStorageModel(component.Control));
                }
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task<List<GroupStorageModel>> GetGroupStorageModelsAsync()
        {
            var resultList = new List<GroupStorageModel>();

            foreach (var component in Components.Select(x => x.Control))
            {
                if (component is ICustomLayoutPane customPane)
                {
                    var storageEntry = new GroupStorageModel
                    {
                        InstanceId = customPane.InstanceId
                    };
                    storageEntry.SubEntries.AddRange(await customPane.GetGroupStorageModelsAsync());
                    resultList.Add(storageEntry);
                }
                else
                {
                    if (component is IGroupingDisplayComponent groupDisplayComponent)
                    {
                        resultList.Add(new()
                        {
                            InstanceId = groupDisplayComponent.InstanceId,
                            ShapingEntries = groupDisplayComponent.GroupingPropertyModule.EntriesShapedBy.Select(x => new GroupEntryStorageModel((GroupShapingEntry)x)).ToList()
                        });
                    }
                }
            }

            return resultList;
        }

        /// <inheritdoc />
        public async Task HandleControlLoadAsync(List<ControlStorageModel> controls, List<SortStorageModel>? sortOrder, List<GroupStorageModel>? groupOrder, List<FilterStorageModel>? filters, List<CustomStorageModel>? customData)
        {
            foreach (var item in controls)
            {
                var newControl = await _controlDiscoveryHelper
                    .CreateControlAsync(item.ContentId)
                    .ConfigureAwait(false);
                if (newControl is ICustomLayoutPane customPane)
                {
                    item.UniqueControlIdentifier = Guid.NewGuid();
                    customPane.UniqueIdentifier = item.UniqueControlIdentifier;
                    await customPane
                        .InitAsync(item.ContentId)
                        .ConfigureAwait(false);
                }

                SetupNewControl(newControl, item, sortOrder, groupOrder, filters, customData);
            }

            await ResizeAllControlsAsync().ConfigureAwait(false);
            await InitializeCustomLayoutPaneAsync(controls, sortOrder, groupOrder, filters, customData).ConfigureAwait(false);
            
        }

        /// <inheritdoc />
        public async Task ResizeControlsWithDragHandleAsync(LayoutDisplay content)
        {
            await HandleControlResizingAsync(content, true).ConfigureAwait(false);
            await CustomLayoutEventRouter
                .SetLayoutChanged()
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task SetControlDisplayAsync()
        {
            foreach (var control in Components)
            {
                control.LayoutBeingEdited = _showControls;
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task SetControlMoveAsync()
        {
            foreach (var control in Components)
            {
                control.CanMove = _moveControls;
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task SetControlResizingAsync()
        {
            foreach (var control in Components)
            {
                control.ShowResizeHandle = _resizeControls;
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task UpdateChildrenSizeAsync(double width, double height)
        {
            var controlHeight = ShowControls
                ? -ManagementControlHeight
                : 0;

            Width = width - BorderWidthToTrim;
            Height = height - BorderHeightToTrim + controlHeight;

            if (Components.Count > 0
                && Width > 0
                && Height > 0)
            {
                await ResizeAllControlsAsync().ConfigureAwait(false);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Occurs when the <see cref="CustomLayoutEventRouter" />'s value changes
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private async void _customLayoutEventRouter_ControlDisplayChanged(object? sender, bool e)
        {
            ShowControls = e;

            Height = e
                ? Height - ManagementControlHeight
                : Height + ManagementControlHeight;
            if (Components.Count > 0)
            {
                await ResizeAllControlsAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Occurs when the <see cref="Components" /> collection changes
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void Components_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (LayoutDisplay entry in e.NewItems ?? new List<LayoutDisplay>(0))
                {
                    entry.RequestRemoval += Entry_RequestRemoval;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (LayoutDisplay entry in e.OldItems ?? new List<LayoutDisplay>(0))
                {
                    entry.RequestRemoval -= Entry_RequestRemoval;
                }
            }
        }

        private void CustomShapingDisplayComponentOnShapingChanged(object? sender, EventArgs e)
        {
            CustomLayoutEventRouter.SetCustomDataChanged();
        }

        /// <summary>
        ///     Occurs when an entry requests removal from the <see cref="Components" /> collection
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private async void Entry_RequestRemoval(object? sender, EventArgs e)
        {
            if (sender is LayoutDisplay displayItem)
            {
                Components.Remove(displayItem);
                if (displayItem.Control is IAsyncDisposable disposableControl)
                {
                    await disposableControl.DisposeAsync();
                }

                await CustomLayoutEventRouter
                    .SetLayoutChanged()
                    .ConfigureAwait(false);
                await ResizeAllControlsAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Occurs when a <see cref="IFilteringDisplayComponent"/> filter changes
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void FilteringDisplayComponentOnFilterChanged(object? sender, EventArgs e)
        {
            CustomLayoutEventRouter.SetFilterChanged();
        }

        /// <summary>
        /// Occurs when a <see cref="IGroupingDisplayComponent"/> group order changed
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void GroupingDisplayComponentOnGroupChanged(object? sender, EventArgs e)
        {
            CustomLayoutEventRouter.SetGroupOrderChanged();
        }

        /// <summary>
        ///     Handle the resizing of children controls
        /// </summary>
        /// <param name="componentBeingResized">The index of the control being resized</param>
        /// <param name="resizeOnlyAfterComponent">
        ///     Indicate if all children should be resized or only the children after the
        ///     component being resized
        /// </param>
        protected abstract Task HandleControlResizingAsync(LayoutDisplay componentBeingResized,
            bool resizeOnlyAfterComponent);

        /// <summary>
        /// Handle setting up of the sorting, filtering and grouping for a new control
        /// </summary>
        /// <param name="newControl">Control entry to set up</param>
        /// <param name="sortOrder">Collection of sort order entries that might contain sort details for the control</param>
        /// <param name="groupOrder">Collection of group order entries that might contain group details for the control</param>
        /// <param name="filters">Collection of filter entries that might contains filter details for the control</param>
        private void HandleShapingOfNewControlEntries(IDisplayComponent? newControl, List<SortStorageModel>? sortOrder, List<GroupStorageModel>? groupOrder, List<FilterStorageModel>? filters, List<CustomStorageModel>? customData)
        {
            if (newControl is ISortableDisplayComponent sortableDisplayComponent)
            {
                var sortDetails = sortOrder?.FirstOrDefault(x => x.InstanceId == newControl.InstanceId);
                if (sortDetails != null)
                {
                    sortableDisplayComponent.InitSortOrder(sortDetails.ShapingEntries);
                }
                sortableDisplayComponent.SortChanged += SortableDisplayComponent_SortChanged;
            }

            if (newControl is IGroupingDisplayComponent groupingDisplayComponent)
            {
                var groupDetails = groupOrder?.FirstOrDefault(x => x.InstanceId == newControl.InstanceId);
                if (groupDetails != null)
                {
                    groupingDisplayComponent.InitGroupOrder(groupDetails.ShapingEntries);
                }
                groupingDisplayComponent.GroupChanged += GroupingDisplayComponentOnGroupChanged;
            }

            if (newControl is IFilteringDisplayComponent filteringDisplayComponent)
            {
                var filterDetails = filters?.FirstOrDefault(x => x.InstanceId == newControl.InstanceId);
                if (filterDetails != null)
                {
                    filteringDisplayComponent.InitFilterOrder(filterDetails.ShapingEntries);
                }
                filteringDisplayComponent.FilterChanged += FilteringDisplayComponentOnFilterChanged;
            }

            if (newControl is ICustomShapingDisplayComponent customShapingDisplayComponent)
            {
                var customDetails = customData?.FirstOrDefault(x => x.InstanceId == newControl.InstanceId);
                if (customDetails != null)
                {
                    customShapingDisplayComponent.InitFilterOrder(customDetails.ShapingEntries);
                }
                customShapingDisplayComponent.ShapingChanged += CustomShapingDisplayComponentOnShapingChanged;
            }
        }

        /// <summary>
        /// Initialize <see cref="ICustomLayoutPane"/> entries in the <see cref="Component"/> collection
        /// </summary>
        /// <param name="controls">Storage models that contains the control details</param>
        /// <param name="sortOrder">Collection of sort order entries that might contain sort details for the control</param>
        /// <param name="groupOrder">Collection of group order entries that might contain group details for the control</param>
        /// <param name="filters">Collection of filter entries that might contain filtering details for the control</param>
        private async Task InitializeCustomLayoutPaneAsync(List<ControlStorageModel> controls, List<SortStorageModel>? sortOrder, List<GroupStorageModel>? groupOrder, List<FilterStorageModel>? filters, List<CustomStorageModel>? customData)
        {
            foreach (var layoutPane in Components.Where(x => x.Control is ICustomLayoutPane).Select(x => x.Control))
            {
                if (layoutPane is ICustomLayoutPane customPane)
                {
                    var sortChildren = sortOrder?.FirstOrDefault(x => x.InstanceId == customPane.InstanceId);
                    var groupChildren = groupOrder?.FirstOrDefault(x => x.InstanceId == customPane.InstanceId);
                    var customChildren = customData?.FirstOrDefault(x => x.InstanceId == customPane.InstanceId);

                    var controlId = customPane.GetIdentifier();
                    var controlItems = controls.First(x => x.UniqueControlIdentifier == customPane.UniqueIdentifier);
                    await customPane
                        .InitAsync(controlId, controlItems.ChildItems, sortChildren?.SubEntries, groupChildren?.SubEntries, filters, customChildren?.SubEntries)
                        .ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        ///     Handle button click from the <see cref="ControlMenu" />
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private async void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem flyoutItem)
            {
                var newControl = await _controlDiscoveryHelper.CreateControlAsync(Guid.Parse(flyoutItem.Name));
                if (newControl != null)
                {
                    var layout = new LayoutDisplay(newControl, Dialogs, CustomLayoutEventRouter)
                    {
                        CanMove = _moveControls,
                        ShowResizeHandle = _resizeControls,
                        LayoutBeingEdited = _showControls
                    };
                    Logger.LogDebug("Adding control {ContentId}", newControl.ContentId);
                    Components.Add(layout);
                    await CustomLayoutEventRouter.SetLayoutChanged().ConfigureAwait(false);
                    await ResizeAllControlsAsync().ConfigureAwait(false);
                    if (newControl is ISortableDisplayComponent sortableDisplayComponent)
                    {
                        sortableDisplayComponent.SortChanged += SortableDisplayComponent_SortChanged;
                    }

                    if (newControl is IGroupingDisplayComponent groupingDisplayComponent)
                    {
                        groupingDisplayComponent.GroupChanged += GroupingDisplayComponentOnGroupChanged;
                    }

                    if (newControl is IFilteringDisplayComponent filteringDisplayComponent)
                    {
                        filteringDisplayComponent.FilterChanged += FilteringDisplayComponentOnFilterChanged;
                    }
                }
            }
        }

        /// <summary>
        ///     Handle the resizing of all controls
        /// </summary>
        protected virtual async Task ResizeAllControlsAsync()
        {
            if (Components.Count > 0)
            {
                await HandleControlResizingAsync(Components[0], false).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Handle the setup of a new control.
        /// </summary>
        /// <param name="newControl">Control entry to set up</param>
        /// <param name="storageModel">Storage model that contains the control details</param>
        /// <param name="sortOrder">Collection of sort order entries that might contain sort details for the control</param>
        /// <param name="groupOrder">Collection of group order entries that might contain group details for the control</param>
        /// <param name="filters">Collection of filter entries that might contains filter details for the control</param>
        private void SetupNewControl(IDisplayComponent? newControl, ControlStorageModel storageModel, List<SortStorageModel>? sortOrder, List<GroupStorageModel>? groupOrder, List<FilterStorageModel>? filters, List<CustomStorageModel>? customData)
        {
            if (newControl != null)
            {
                newControl.InstanceId = storageModel.InstanceId == Guid.Empty
                    ? Guid.NewGuid()
                    : storageModel.InstanceId;

                HandleShapingOfNewControlEntries(newControl, sortOrder, groupOrder, filters, customData);

                newControl.Title = storageModel.Title;
                var layout = new LayoutDisplay(newControl, Dialogs, CustomLayoutEventRouter)
                {
                    CanMove = _moveControls,
                    ShowResizeHandle = _resizeControls,
                    LayoutBeingEdited = _showControls,
                    Control =
                    {
                        Width = storageModel.Width,
                        Height = storageModel.Height
                    }
                };

                Logger.LogDebug("Adding control {ControlId}", newControl.ContentId);
                Components.Add(layout);
            }
        }

        /// <summary>
        /// Occurs when a <see cref="ISortableDisplayComponent"/> sort order changed
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event argument</param>
        private void SortableDisplayComponent_SortChanged(object? sender, EventArgs e)
        {
            CustomLayoutEventRouter.SetSortOrderChanged();
        }

        /// <summary>
        ///     Update the size of children components to the ones provided
        /// </summary>
        /// <param name="childrenSizes">Size to set the child components to. Note that the order should match the children`s order</param>
        /// <param name="startIndex">Index to start resizing from</param>
        protected virtual async Task UpdateChildrenSizeAsync(List<Size> childrenSizes, int startIndex)
        {
            var childrenCount = Components.Count;
            for (var r = startIndex; r < childrenCount; r++)
            {
                var child = (UserControl) Components[r].Control;
                var adjustedWidth = childrenSizes[r].Width;
                var adjustedHeight = childrenSizes[r].Height;
                if (child is ICustomLayoutPane customChild)
                {
                    await customChild
                        .SetControlSizeAsync(adjustedWidth, adjustedHeight)
                        .ConfigureAwait(false);
                }
                else
                {
                    child.Width = adjustedWidth;
                    child.Height = adjustedHeight;
                }
            }
        }

        #endregion

        #region Variables

        /// <summary>
        ///     Height to add when the pane management controls are active
        /// </summary>
        private const int ManagementControlHeight = 50;

        /// <summary>
        ///     Size to trim from each control's height
        /// </summary>
        private const int BorderHeightToTrim = 10;

        /// <summary>
        ///     Size to trim from each control
        /// </summary>
        private const int BorderWidthToTrim = 10;

        /// <summary>
        ///     ControlDiscoveryHelper instance
        /// </summary>
        private readonly IControlDiscoveryHelper _controlDiscoveryHelper;

        /// <summary>
        /// CustomLayoutEventRouter instance
        /// </summary>
        protected readonly ICustomLayoutEventRouter CustomLayoutEventRouter;

        /// <summary>
        /// Dialogs instance
        /// </summary>
        protected readonly IDialogs Dialogs;

        /// <summary>
        /// Logger instance
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Backing variable for the <see cref="ControlMenu" /> property
        /// </summary>
        private MenuFlyout? _menuFlyout;

        /// <summary>
        /// Backing variable for <see cref="MoveControls" />
        /// </summary>
        private bool _moveControls;

        /// <summary>
        /// Backing variable for <see cref="ResizeControls" />
        /// </summary>
        private bool _resizeControls;

        /// <summary>
        /// Backing variable for <see cref="ShowControls" />
        /// </summary>
        private bool _showControls;

        /// <summary>
        /// Height of the parent container
        /// </summary>
        protected double Height;

        /// <summary>
        /// Width of the parent container
        /// </summary>
        protected double Width;

        #endregion
    }
}