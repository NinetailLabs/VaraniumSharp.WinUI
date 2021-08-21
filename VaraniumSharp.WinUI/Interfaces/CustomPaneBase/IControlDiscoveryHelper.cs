using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.WinUI.CustomPaneBase;

namespace VaraniumSharp.WinUI.Interfaces.CustomPaneBase
{
    /// <summary>
    /// Helps with the discovery and creation of controls that implement <see cref="IDisplayComponent"/> and is decorated with <see cref="DisplayComponentAttribute"/>
    /// </summary>
    public interface IControlDiscoveryHelper
    {
         #region Properties

        /// <summary>
        /// Controls that are available for creation
        /// </summary>
        Dictionary<Guid, DisplayComponentAttribute> AvailableControls { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a control based on its Title
        /// </summary>
        /// <param name="contentId">The ContentId of the control to load</param>
        /// <returns></returns>
        Task<IDisplayComponent?> CreateControlAsync(Guid contentId);

        /// <summary>
        /// Retrieve a <see cref="MenuFlyout"/> that contains all control that can be created.
        /// This is useful for custom layout panes that need a collection of control
        /// </summary>
        /// <param name="buttonClickAction">Action that should be invoked when a control creation button is clicked</param>
        /// <returns>MenuItem containing available controls</returns>
        MenuFlyout GetMenuItemWithAvailableControls(Action<object, RoutedEventArgs> buttonClickAction);

        #endregion
    }
}