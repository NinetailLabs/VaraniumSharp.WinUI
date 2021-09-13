using System;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;

namespace VaraniumSharp.WinUI.CustomPaneBase
{
    /// <summary>
    /// Attribute to decorate <see cref="IDisplayComponent"/> implementers that should be available as custom controls
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DisplayComponentAttribute : Attribute
    {
        #region Constructor

        /// <summary>
        /// Construct for use with CustomLayout but without a Ribbon Button
        /// </summary>
        /// <param name="controlName">The name of the control</param>
        /// <param name="contentId">The Id of the content</param>
        /// <param name="subMenu">The submenu of the flyout the item appears in</param>
        /// <param name="minWidth">The minimum width of the control</param>
        /// <param name="minHeight">The minimum height of the control</param>
        /// <param name="registeredInterface">The interface with which the control is registered in the DI container</param>
        public DisplayComponentAttribute(string controlName, string contentId, string subMenu, double minWidth, double minHeight, Type registeredInterface)
        {
            ControlName = controlName;
            ContentId = contentId;
            SubMenu = subMenu;
            RegisteredInterface = registeredInterface;
            MinWidth = minWidth;
            MinHeight = minHeight;
            ShowInMenus = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The content Id of the control
        /// </summary>
        public string ContentId { get; }

        /// <summary>
        /// The display name for the control
        /// </summary>
        public string ControlName { get; }

        /// <summary>
        /// The minimum height that the control can be
        /// </summary>
        public double MinHeight { get; }

        /// <summary>
        /// The minimum width that the control can be
        /// </summary>
        public double MinWidth { get; }

        /// <summary>
        /// The interface that should be used to retrieve the control from the DI container
        /// </summary>
        public Type RegisteredInterface { get; }

        /// <summary>
        /// Indicate if the control should be shown in the flyout menu to be user addable
        /// </summary>
        public bool ShowInMenus { get; set; }

        /// <summary>
        /// SubMenu where the item appears in the flyout
        /// </summary>
        public string SubMenu { get; }

        #endregion
    }
}