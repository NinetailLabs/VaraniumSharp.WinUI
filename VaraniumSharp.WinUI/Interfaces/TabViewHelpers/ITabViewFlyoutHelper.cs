using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using System;

namespace VaraniumSharp.WinUI.Interfaces.TabViewHelpers
{
    /// <summary>
    /// Logic that can be used to generate a <see cref="MenuFlyout"/> for a <see cref="TabViewItem"/>
    /// </summary>
    public interface ITabViewFlyoutHelper
    {
        #region Public Methods

        /// <summary>
        /// Create a <see cref="MenuFlyout"/> for a TabViewItem
        /// </summary>
        /// <param name="tabItem">Item the flyout should be created for</param>
        /// <returns>Created flyout</returns>
        MenuFlyout CreateFlyoutForTabItem(TabViewItem tabItem);

        /// <summary>
        /// Set the action to be called when changes to tabs occurred so that the changes can be persisted
        /// </summary>
        /// <param name="saveCallbackFuncAsync">Action to be called</param>
        void SetSaveCallback(Func<Task> saveCallbackFuncAsync);

        #endregion
    }
}
