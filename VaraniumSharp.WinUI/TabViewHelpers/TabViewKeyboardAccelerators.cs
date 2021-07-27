using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Threading.Tasks;
using Windows.System;

namespace VaraniumSharp.WinUI.TabViewHelpers
{
    /// <summary>
    /// Keyboard accelerator methods for use with a <see cref="TabView"/>
    /// </summary>
    public sealed class TabViewKeyboardAccelerators
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="setIndexFunc"></param>
        /// <param name="removeTabFunc"></param>
        /// <param name="addTabFunc"></param>
        /// <param name="getTabCount"></param>
        public TabViewKeyboardAccelerators(Func<int, Task> setIndexFunc, Func<TabViewItem, Task> removeTabFunc, Func<Task> addTabFunc, Func<int> getTabCount)
        {
            _setIndexFuncAsync = setIndexFunc;
            _removeTabFuncAsync = removeTabFunc;
            _addTabFuncAsync = addTabFunc;
            GetTabCount = getTabCount;
        }

        /// <summary>
        /// Keyboard accelerator for adding a new Tab
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="args">Event arguments</param>
        public async void OnNewTabKeyboardAccelerator(KeyboardAccelerator? sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            await _addTabFuncAsync();
            args.Handled = true;
        }

        /// <summary>
        /// Keyboard accelerator for closing a tab
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="args">Event arguments</param>
        public async void OnCloseTabKeyboardAccelerator(KeyboardAccelerator? sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (args.Element is TabView invokedTabView
                && invokedTabView.SelectedItem is TabViewItem tab
                && tab.IsClosable)
            {
                await _removeTabFuncAsync(tab);
            }

            args.Handled = true;
        }

        /// <summary>
        /// Keyboard accelerator for handling tab switching
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="args">Event arguments</param>
        public async void OnNavigateToTabKeyboardAccelerator(KeyboardAccelerator? sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var tabToSelect = 0;

            switch (sender?.Key)
            {
                case VirtualKey.Number1:
                    tabToSelect = 0;
                    break;
                case VirtualKey.Number2:
                    tabToSelect = 1;
                    break;
                case VirtualKey.Number3:
                    tabToSelect = 2;
                    break;
                case VirtualKey.Number4:
                    tabToSelect = 3;
                    break;
                case VirtualKey.Number5:
                    tabToSelect = 4;
                    break;
                case VirtualKey.Number6:
                    tabToSelect = 5;
                    break;
                case VirtualKey.Number7:
                    tabToSelect = 6;
                    break;
                case VirtualKey.Number8:
                    tabToSelect = 7;
                    break;
                case VirtualKey.Number9:
                    // Select the last tab
                    tabToSelect = GetTabCount() - 1;
                    break;
            }

            await _setIndexFuncAsync(tabToSelect);

            args.Handled = true;
        }

        /// <summary>
        /// Function to invoke to set the SelectedIndex for tab items
        /// </summary>
        private readonly Func<int, Task> _setIndexFuncAsync;

        /// <summary>
        /// Function to invoke to remove a tab item
        /// </summary>
        private readonly Func<TabViewItem, Task> _removeTabFuncAsync;

        /// <summary>
        /// Function to invoke to add a new tab item
        /// </summary>
        private readonly Func<Task> _addTabFuncAsync;

        /// <summary>
        /// Function to invoke to get the number of Tabs
        /// </summary>
        public Func<int> GetTabCount { get; }
    }
}
