using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using VaraniumSharp.Attributes;
using VaraniumSharp.Interfaces.Wrappers;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.Dialogs;
using VaraniumSharp.WinUI.Interfaces.TabViewHelpers;
using VaraniumSharp.WinUI.Interfaces.TabWindow;
using VaraniumSharp.WinUI.TabViewHelpers;

namespace VaraniumSharp.WinUI.TabWindow
{
    /// <summary>
    /// Context for TabView window
    /// </summary>
    [AutomaticContainerRegistration(typeof(ITabWindowContext))]
    public sealed class TabWindowContext : ITabWindowContext
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public TabWindowContext(IDialogs dialogs, IFileWrapper fileWrapper, ITabViewFlyoutHelper tabViewFlyoutHelper, ITabViewStorageManager tabViewStorageManager,
            IContentPaneManager contentPaneManager, ILayoutStorageOptions layoutStorageOptions, ICustomLayoutEventRouter customLayoutEventRouter)
        {
            _dialogs = dialogs;
            _fileWrapper = fileWrapper;
            _tabViewItemFlyoutHelper = tabViewFlyoutHelper;
            _tabViewStorageManager = tabViewStorageManager;
            _layoutStorageOptions = layoutStorageOptions;
            ContentPaneManager = contentPaneManager;
            _tabViewItemFlyoutHelper.SetSaveCallback(HandleTabViewPersistenceAsync);
            customLayoutEventRouter.LayoutChanged += CustomLayoutEventRouterOnLayoutChanged;

            Tabs = new ObservableCollection<TabViewItem>();
            Tabs.CollectionChanged += Tabs_CollectionChanged;
            KeyboardAccelerators = new TabViewKeyboardAccelerators(HandleIndexSetAsync, HandleTabRemovalAsync, AddTabAsync, () => Tabs.Count);
        }

        #endregion

        #region Events

        /// <inheritdoc />
#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        #endregion

        #region Properties

        /// <inheritdoc/>
        public IContentPaneManager ContentPaneManager { get; }

        /// <inheritdoc/>
        public bool EnableContextMenuItems { get; private set; }

        /// <inheritdoc/>
        public TabViewKeyboardAccelerators KeyboardAccelerators { get; set; }

        /// <inheritdoc/>
        public double MaxTabViewSize { get; set; }

        /// <inheritdoc/>
        public int SelectedIndex { get; set; }

        /// <inheritdoc/>
        public ObservableCollection<TabViewItem> Tabs { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public async void OnAddClickedAsync(TabView? sender, object args)
        {
            await AddTabAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async void OnLoadedAsync(object sender, RoutedEventArgs e)
        {
            _loading = true;

            var filePath = _layoutStorageOptions.GetJsonPath(LayoutFile);
            if (!_fileWrapper.FileExists(filePath))
            {
                await AddTabAsync().ConfigureAwait(true);
                await HandleTabViewPersistenceAsync().ConfigureAwait(true);
            }
            else
            {
                try
                {
                    var tabs = await _tabViewStorageManager
                        .LoadLayoutAsync(filePath)
                        .ConfigureAwait(true);
                    foreach (var tab in tabs)
                    {
                        await AddExistingTabAsync(tab.GetTabViewItem()).ConfigureAwait(true);
                    }
                }
                catch (Exception)
                {
                    if (_root != null)
                    {
                        await _dialogs
                            .ShowMessageDialogAsync("Error", $"An error occurred while loading the Tab layout. Please ensure the layout file is accessible and valid.\r\n\r\n{filePath}", _root)
                            .ConfigureAwait(true);
                    }
                }
            }

            _loading = false;
        }

        /// <inheritdoc/>
        public async void OnSelectionChangedAsync(object? sender, SelectionChangedEventArgs e)
        {
            if (SelectedIndex < 0 || SelectedIndex >= Tabs.Count)
            {
                return;
            }

            var tab = Tabs[SelectedIndex];
            if (tab.Name == SettingGuid)
            {
                EnableContextMenuItems = false;
                await ShowSettingPaneAsync().ConfigureAwait(true);
                return;
            }

            EnableContextMenuItems = true;
            await HandleTabChangesAsync().ConfigureAwait(true);
            await ContentPaneManager
                .UpdateContentAsync(tab.Name)
                .ConfigureAwait(true);
            _previousIndex = SelectedIndex;
        }

        /// <inheritdoc/>
        public async void OnTabClosedAsync(TabView? sender, TabViewTabCloseRequestedEventArgs args)
        {
            await HandleTabRemovalAsync(args.Tab).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task SaveLayoutAsync()
        {
            await ContentPaneManager
                .SaveLayoutAsync()
                .ConfigureAwait(true);
            Tabs[SelectedIndex].IconSource = null;
            _currentTabHasChanges = false;
        }

        /// <inheritdoc/>
        public void SetXamlRoot(XamlRoot root)
        {
            _root = root;
        }

        /// <inheritdoc/>
        public async Task ShowSettingPaneAsync()
        {
            await HandleTabChangesAsync().ConfigureAwait(true);

            if (Tabs.Any(x => x.Name == SettingGuid))
            {
                var settingTab = Tabs.First(x => x.Name == SettingGuid);
                SelectedIndex = Tabs.IndexOf(settingTab);
            }
            else
            {
                var settingTab = new TabViewItem
                {
                    Name = SettingGuid,
                    Header = "Settings"
                };
                Tabs.Add(settingTab);
                SelectedIndex = Tabs.IndexOf(settingTab);
            }

            await ContentPaneManager
                .ShowSettingPageAsync()
                .ConfigureAwait(true);
            _previousIndex = SelectedIndex;
        }

        /// <summary>
        /// Occurs when the size of the outer grid changes
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        public void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var size = e.NewSize;
            var newWidth = size.Width - 170; // 120 for button + 50 for titlebar "handle"
            MaxTabViewSize = newWidth > 0
                ? newWidth
                : 0;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Add an existing tab to the <see cref="Tabs"/> collection
        /// </summary>
        /// <param name="tab">The tab to add</param>
        private Task AddExistingTabAsync(TabViewItem tab)
        {
            if (tab.Name != SettingGuid)
            {
                tab.ContextFlyout = _tabViewItemFlyoutHelper.CreateFlyoutForTabItem(tab);
            }

            Tabs.Add(tab);
            if (Tabs.Count == 1)
            {
                SelectedIndex = 0;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Add a new Tab to the <see cref="Tabs"/> collection
        /// </summary>
        private async Task AddTabAsync()
        {
            Tabs.Add(await CreateNewTabAsync().ConfigureAwait(true));
            if (Tabs.Count == 1)
            {
                SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Create a new TabViewItem
        /// </summary>
        /// <returns>Initialized TabViewItem</returns>
        private Task<TabViewItem> CreateNewTabAsync()
        {
            var newItem = new TabViewItem
            {
                Name = Guid.NewGuid().ToString(),
                Header = "New Tab"
            };
            newItem.ContextFlyout = _tabViewItemFlyoutHelper.CreateFlyoutForTabItem(newItem);

            return Task.FromResult(newItem);
        }

        /// <summary>
        /// Occurs when a custom layout changed.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void CustomLayoutEventRouterOnLayoutChanged(object? sender, EventArgs e)
        {
            if (!_currentTabHasChanges)
            {
                _currentTabHasChanges = true;
                Tabs[SelectedIndex].IconSource = new FontIconSource
                {
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 185, 0)),
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    FontSize = 12,
                    Glyph = "\ue915"
                };
            }
        }

        /// <summary>
        /// Handle the setting of <see cref="SelectedIndex"/> value
        /// </summary>
        /// <param name="indexToSelect">The index to select</param>
        private Task HandleIndexSetAsync(int indexToSelect)
        {
            if (indexToSelect < Tabs.Count)
            {
                SelectedIndex = indexToSelect;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handle tab if it has changes.
        /// </summary>
        private async Task HandleTabChangesAsync()
        {
            if (_currentTabHasChanges)
            {
                if (_root != null 
                    && await _dialogs
                        .ShowConfirmationDialog("Unsaved Changes", "Your layout has unsaved changes.\r\nDo you want to save them?", _root)
                        .ConfigureAwait(true))
                {
                    await ContentPaneManager
                        .SaveLayoutAsync()
                        .ConfigureAwait(true);
                }
                Tabs[_previousIndex].IconSource = null;
                _currentTabHasChanges = false;
            }
        }

        /// <summary>
        /// Handle the removal of a Tab item.
        /// The caller is responsible for checking if the item can be removed
        /// </summary>
        /// <param name="tabToRemove">The tab to remove</param>
        private async Task HandleTabRemovalAsync(TabViewItem tabToRemove)
        {
            if (tabToRemove.IsClosable)
            {
                Tabs.Remove(tabToRemove);
                if (tabToRemove.Content is IAsyncDisposable disposableContent)
                {
                    await disposableContent
                        .DisposeAsync()
                        .ConfigureAwait(true);
                }

                var paneIdentifier = Guid.Parse(tabToRemove.Name);
                await ContentPaneManager
                    .DeleteLayoutAsync(paneIdentifier)
                    .ConfigureAwait(true);
            }
            await HandleTabViewPersistenceAsync().ConfigureAwait(true);
        }

        /// <summary>
        /// Store the TabView details to file
        /// </summary>
        private async Task HandleTabViewPersistenceAsync()
        {
            var tabCollection = Tabs
                .Select(tab => new TabViewModel(tab));

            var file = _layoutStorageOptions.GetJsonPath(LayoutFile);

            try
            {
                await _tabViewStorageManager
                    .SaveLayoutAsync(tabCollection, file)
                    .ConfigureAwait(true);
            }
            catch (Exception)
            {
                if (_root != null)
                {
                    await _dialogs
                        .ShowMessageDialogAsync("Error", $"An error occurred while attempting to save the Tab layout.\r\nPlease ensure that the layout file is accessible.\r\n\r\n{file}", _root)
                        .ConfigureAwait(true);
                }
            }
        }

        /// <summary>
        /// Fired when the <see cref="Tabs"/> collection changes
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private async void Tabs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_loading
                && e.Action is NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Add)
            {
                await HandleTabViewPersistenceAsync().ConfigureAwait(false);
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// Name of the tab layout file
        /// </summary>
        private const string LayoutFile = "TabLayout.json";

        /// <summary>
        /// The GUID for the Settings pane
        /// </summary>
        private const string SettingGuid = "90ef7c67-1cea-4001-aedc-afb8c760a4c8";

        /// <summary>
        /// Dialogs instance
        /// </summary>
        private readonly IDialogs _dialogs;

        /// <summary>
        /// FileWrapper instance
        /// </summary>
        private readonly IFileWrapper _fileWrapper;

        /// <summary>
        /// LayoutStorageOptions instance
        /// </summary>
        private readonly ILayoutStorageOptions _layoutStorageOptions;

        /// <summary>
        /// TabViewItemFlyoutHelper instance
        /// </summary>
        private readonly ITabViewFlyoutHelper _tabViewItemFlyoutHelper;

        /// <summary>
        /// TabViewItemStorageManager instance
        /// </summary>
        private readonly ITabViewStorageManager _tabViewStorageManager;

        /// <summary>
        /// Indicates if the current tab has unsaved changes
        /// </summary>
        private bool _currentTabHasChanges;

        /// <summary>
        /// Indicate if the collection is being loaded
        /// </summary>
        private bool _loading;

        /// <summary>
        /// The previously selected index
        /// </summary>
        private int _previousIndex;

        /// <summary>
        /// The XamlRoot of the container
        /// </summary>
        private XamlRoot? _root;

        #endregion
    }
}