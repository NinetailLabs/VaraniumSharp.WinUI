using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.Interfaces.Dialogs;
using VaraniumSharp.WinUI.Interfaces.TabViewHelpers;

namespace VaraniumSharp.WinUI.TabViewHelpers
{
    /// <summary>
    /// Logic that can be used to generate a <see cref="MenuFlyout"/> for a <see cref="TabViewItem"/>
    /// </summary>
    [AutomaticContainerRegistration(typeof(ITabViewFlyoutHelper))]
    public sealed class TabViewItemFlyoutHelper : ITabViewFlyoutHelper
    {
        /// <summary>
        /// DI Constructor
        /// </summary>
        public TabViewItemFlyoutHelper(IDialogs dialogs)
        {
            _dialogHelper = dialogs;
        }

        /// <inheritdoc/>
        public void SetSaveCallback(Func<Task> saveCallbackFuncAsync)
        {
            _saveCallbackFuncAsync = saveCallbackFuncAsync;
        }

        /// <inheritdoc/>
        public MenuFlyout CreateFlyoutForTabItem(TabViewItem tabItem)
        {
            var flyout = new MenuFlyout();
            var renameItem = new MenuFlyoutItem
            {
                Text = "Rename",
                DataContext = tabItem
            };
            renameItem.Click += RenameItem;

            var tabCloseItem = new MenuFlyoutSubItem
            {
                Text = "Tab Close"
            };
            var canCloseItem = new RadioMenuFlyoutItem
            {
                Text = "Can Close",
                GroupName = "CloseLogic",
                IsChecked = true,
                DataContext = tabItem
            };
            var cannotCloseItem = new RadioMenuFlyoutItem
            {
                Text = "Cannot Close",
                GroupName = "CloseLogic",
                DataContext = tabItem,
            };
            canCloseItem.Click += OnCloseSubItemClick;
            cannotCloseItem.Click += OnCloseSubItemClick;
            tabCloseItem.Items.Add(canCloseItem);
            tabCloseItem.Items.Add(cannotCloseItem);

            flyout.Items.Add(renameItem);
            flyout.Items.Add(tabCloseItem);

            return flyout;
        }

        /// <summary>
        /// Change the <see cref="TabViewItem.IsClosable"/> property based on the item that was clicked
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private async void OnCloseSubItemClick(object? sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuFlyoutItem menuFlyout)
            {
                if (menuFlyout.DataContext is TabViewItem tabItem)
                {
                    if (menuFlyout.Text == "Can Close")
                    {
                        tabItem.IsClosable = true;
                    }
                    else
                    {
                        tabItem.IsClosable = false;
                    }

                    if (_saveCallbackFuncAsync != null)
                    {
                        await _saveCallbackFuncAsync
                            .Invoke()
                            .ConfigureAwait(false);
                    }
                }
            }
        }

        /// <summary>
        /// Rename a TabItem
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private async void RenameItem(object? sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuFlyoutItem menuFlyout)
            {
                if (menuFlyout.DataContext is TabViewItem tabItem)
                {
                    var newHeader = await _dialogHelper
                        .ShowTextInputDialogAsync($"Enter new name for \"{tabItem.Header}\"", tabItem.Header?.ToString() ?? string.Empty, menuFlyout.XamlRoot)
                        .ConfigureAwait(true);
                    if (!string.IsNullOrEmpty(newHeader))
                    {
                        tabItem.Header = newHeader;

                        if (_saveCallbackFuncAsync != null)
                        {
                            await _saveCallbackFuncAsync
                                .Invoke()
                                .ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// DialogHelper instance
        /// </summary>
        private readonly IDialogs _dialogHelper;

        /// <summary>
        /// Function to call when changes were made to a tab to request persistence
        /// </summary>
        private Func<Task>? _saveCallbackFuncAsync;
    }
}
