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
        #region Constructor

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

        #endregion

        #region Properties

        /// <summary>
        /// Function to invoke to get the number of Tabs
        /// </summary>
        public Func<int> GetTabCount { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Keyboard accelerator for closing a tab
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="args">Event arguments</param>
        public async void OnCloseTabKeyboardAccelerator(KeyboardAccelerator? sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (args.Element is TabView {SelectedItem: TabViewItem {IsClosable: true} tab})
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
            var tabToSelect = sender?.Key switch
            {
                VirtualKey.Number1 => 0,
                VirtualKey.Number2 => 1,
                VirtualKey.Number3 => 2,
                VirtualKey.Number4 => 3,
                VirtualKey.Number5 => 4,
                VirtualKey.Number6 => 5,
                VirtualKey.Number7 => 6,
                VirtualKey.Number8 => 7,
                VirtualKey.Number9 =>
                    // Select the last tab
                    GetTabCount() - 1,
                _ => 0
            };

            await _setIndexFuncAsync(tabToSelect);

            args.Handled = true;
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

        #endregion

        #region Variables

        /// <summary>
        /// Function to invoke to add a new tab item
        /// </summary>
        private readonly Func<Task> _addTabFuncAsync;

        /// <summary>
        /// Function to invoke to remove a tab item
        /// </summary>
        private readonly Func<TabViewItem, Task> _removeTabFuncAsync;

        /// <summary>
        /// Function to invoke to set the SelectedIndex for tab items
        /// </summary>
        private readonly Func<int, Task> _setIndexFuncAsync;

        #endregion
    }
}
