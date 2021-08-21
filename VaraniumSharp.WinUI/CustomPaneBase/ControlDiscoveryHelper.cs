using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.Attributes;
using VaraniumSharp.Enumerations;
using VaraniumSharp.Interfaces.DependencyInjection;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;

namespace VaraniumSharp.WinUI.CustomPaneBase
{
    /// <summary>
    /// Assists with the discovery and creation of controls
    /// </summary>
    [AutomaticContainerRegistration(typeof(IControlDiscoveryHelper), ServiceReuse.Singleton)]
    public class ControlDiscoveryHelper : IControlDiscoveryHelper
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public ControlDiscoveryHelper(IContainerFactoryWrapper container)
        {
            _container = container;
            AvailableControls = new();
            DiscoverControls();
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public Dictionary<Guid, DisplayComponentAttribute> AvailableControls { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public async Task<IDisplayComponent?> CreateControlAsync(Guid contentId)
        {
            var controlToCreate = AvailableControls.First(x => x.Key.Equals(contentId));
            var control = (IDisplayComponent)_container.Resolve(controlToCreate.Value.RegisteredInterface);
            control.Width = controlToCreate.Value.MinWidth;
            control.Height = controlToCreate.Value.MinHeight;
            await control
                .InitAsync()
                .ConfigureAwait(false);
            return control;
        }

        /// <inheritdoc/>
        public MenuFlyout GetMenuItemWithAvailableControls(Action<object, RoutedEventArgs> buttonClickAction)
        {
            var menu = new MenuFlyout();

            var subMenuGroup = AvailableControls.GroupBy(x => x.Value.SubMenu);
            foreach (var subMenuItem in subMenuGroup)
            {
                var subMenu = new MenuFlyoutSubItem
                {
                    Name = subMenuItem.Key.Replace(" ", ""),
                    Text = subMenuItem.Key
                };

                foreach (var control in subMenuItem.OrderBy(x => x.Value.ControlName))
                {
                    var menuItem = new MenuFlyoutItem
                    {
                        Name = control.Value.ContentId,
                        Text = control.Value.ControlName
                    };
                    menuItem.Click += buttonClickAction.Invoke;

                    subMenu.Items.Add(menuItem);
                }

                menu.Items.Add(subMenu);
            }

            return menu;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Discover all controls that are available for display
        /// </summary>
        private void DiscoverControls()
        {
            var workableClasses = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && x.GetInterfaces().Contains(typeof(IDisplayComponent))
                && x.GetCustomAttribute(typeof(DisplayComponentAttribute), false) != null);

            foreach (var @class in workableClasses)
            {
                var attribute = (DisplayComponentAttribute?)@class.GetCustomAttribute(typeof(DisplayComponentAttribute), false);

                if (attribute != null)
                {
                    AvailableControls.Add(Guid.Parse(attribute.ContentId), attribute);
                }
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// Container instance
        /// </summary>
        private readonly IContainerFactoryWrapper _container;

        #endregion
    }
}