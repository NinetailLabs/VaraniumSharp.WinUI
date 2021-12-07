using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.WinUI.ExtensionMethods;

namespace VaraniumSharp.WinUI.FilterModule.Controls
{
    /// <summary>
    /// Control used to filter based on a boolean value
    /// </summary>
    public sealed partial class DropDownBoolFilter : IFilterControl, INotifyPropertyChanged
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="propertyName">Text to display for the filter</param>
        /// <param name="displayName">The label to display for the filter textBox</param>
        /// <param name="tooltip">The tooltip to display for the UI components</param>
        public DropDownBoolFilter(string propertyName, string displayName, string tooltip)
        {
            InitializeComponent();
            FilterDisplayName = displayName;
            FilterPropertyName = propertyName;
            FilterTooltip = tooltip;
            FilterBy(new List<object> {BooleanFilterValues.All});
        }

        #endregion

        #region Events

        /// <inheritdoc />
#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        /// <inheritdoc />
        public event EventHandler? RefreshFiltering;

        #endregion

        #region Properties

        /// <inheritdoc />
        public string FilterDisplayName { get; }

        /// <summary>
        /// Name of the property that is being filtered on
        /// </summary>
        public string FilterPropertyName { get; }

        /// <summary>
        /// The tooltip to display for the UI components
        /// </summary>
        public string FilterTooltip { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public bool Filter(object obj)
        {
            if (_selectedFilterValues == BooleanFilterValues.All)
            {
                return true;
            }

            var property = obj.GetNestedPropertyValue(FilterPropertyName);
            return property is bool boolToFilter
                   && (_selectedFilterValues == BooleanFilterValues.Yes && boolToFilter
                       || _selectedFilterValues == BooleanFilterValues.No && !boolToFilter);
        }

        /// <inheritdoc />
        public List<KeyValuePair<object, FilterState>> FilterBy(List<object> entries)
        {
            if (_menu == null)
            {
                GetContextMenu();
            }

            var response = new List<KeyValuePair<object, FilterState>>();

            for (var r = 0; r < entries.Count; r++)
            {
                var entry = entries[r];
                if (r > 0)
                {
                    response.Add(new KeyValuePair<object, FilterState>(entry, FilterState.Ignored));
                    continue;
                }

                try
                {
                    var filterValue = entry.ToString();
                    var menuItem = _menu!.Items
                        .Select(x => (ToggleMenuFlyoutItem)x)
                        .First(x => x.Text.Equals(filterValue, StringComparison.InvariantCultureIgnoreCase));
                    if (entry is not BooleanFilterValues boolFilter)
                    {
                        response.Add(new KeyValuePair<object, FilterState>(entry, FilterState.NotFound));
                        continue;
                    }

                    ApplyFilter(menuItem, boolFilter);
                    response.Add(new KeyValuePair<object, FilterState>(entry, FilterState.Applied));
                }
                catch (Exception)
                {
                    response.Add(new KeyValuePair<object, FilterState>(entry, FilterState.NotValid));
                }
            }

            return response;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Set the filter for the control.
        /// This include both setting the filter and cleaning up/setting filter indicator in the <see cref="_menu"/>
        /// </summary>
        /// <param name="menuItem">The menuItem that the filter is for</param>
        /// <param name="filterValue">Filter value to set</param>
        private void ApplyFilter(ToggleMenuFlyoutItem menuItem, BooleanFilterValues filterValue)
        {
            foreach (var item in _menu?.Items ?? new List<MenuFlyoutItemBase>())
            {
                if (item is ToggleMenuFlyoutItem toggleMenu)
                {
                    toggleMenu.IsChecked = false;
                }
            }

            _selectedFilterValues = filterValue;
            menuItem.IsChecked = true;

            RefreshFiltering?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the button is clicked
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void ButtonClicked(object sender, RoutedEventArgs e)
        {
            var menu = GetContextMenu();
            menu.ShowAt(FlyoutButton);
        }

        /// <summary>
        /// Occurs when the users clicks one of the filter entries
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event argument</param>
        /// <exception cref="NotImplementedException"></exception>
        private void EntryOnClick(object sender, RoutedEventArgs e)
        {
            var menuItem = (ToggleMenuFlyoutItem) sender;
            ApplyFilter(menuItem, Enum.Parse<BooleanFilterValues>(menuItem.Text));
        }

        /// <summary>
        /// Get the context menu that contains the filtering options
        /// </summary>
        /// <returns>Menu with filtering options</returns>
        private MenuFlyout GetContextMenu()
        {
            if (_menu != null)
            {
                return _menu;
            }

            _menu = new MenuFlyout();

            var filterValues = Enum.GetValues<BooleanFilterValues>();
            foreach (var filterValue in filterValues)
            {
                var entry = new ToggleMenuFlyoutItem
                {
                    Text = filterValue.ToString()
                };
                entry.Click += EntryOnClick;

                _menu.Items.Add(entry);
            }

            return _menu;
        }

        #endregion

        #region Variables

        /// <summary>
        /// The context menu containing the filter options
        /// </summary>
        private MenuFlyout? _menu;

        /// <summary>
        /// The currently selected filter
        /// </summary>
        private BooleanFilterValues _selectedFilterValues;

        #endregion
    }
}
