using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using VaraniumSharp.WinUI.ExtensionMethods;

namespace VaraniumSharp.WinUI.FilterModule.Controls
{
    /// <summary>
    /// Control used to filter based on a string list
    /// </summary>
    public sealed partial class DropDownStringFilter : IFilterControl, INotifyPropertyChanged
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="filterShapingEntry">Entry that contains details about the filter</param>
        /// <param name="filterValues">Values that should be filtered by</param>
        public DropDownStringFilter(FilterShapingEntry filterShapingEntry, List<string> filterValues)
        {
            InitializeComponent();
            ShapingEntry = filterShapingEntry;
            _filterValues = filterValues;
            _selectedFilters = new();
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
        public FilterShapingEntry ShapingEntry { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public bool Filter(object obj)
        {
            if (_selectedFilters.Count == 0)
            {
                return true;
            }

            var property = obj.GetNestedPropertyValue(ShapingEntry.PropertyName);
            return property is string stringToFilter
                   && _selectedFilters.Contains(stringToFilter);
        }

        /// <inheritdoc />
        public List<KeyValuePair<object, FilterState>> FilterBy(List<object> entries)
        {
            if (_menu == null)
            {
                GetContextMenu();
            }

            var response = new List<KeyValuePair<object, FilterState>>();
            foreach (var entry in entries)
            {
                try
                {
                    var filterValue = entry.ToString() ?? string.Empty;
                    var menuItem = _menu!.Items
                        .Select(x => (ToggleMenuFlyoutItem)x)
                        .First(x => x.Text.Equals(filterValue, StringComparison.InvariantCultureIgnoreCase));
                    if (!_filterValues.Contains(filterValue) || menuItem == null)
                    {
                        response.Add(new KeyValuePair<object, FilterState>(entry, FilterState.NotFound));
                        continue;
                    }

                    var result = ApplyFilter(menuItem, filterValue);
                    response.Add(new KeyValuePair<object, FilterState>(entry, result));
                }
                catch (Exception)
                {
                    response.Add(new KeyValuePair<object, FilterState>(entry, FilterState.NotValid));
                }
            }

            return response;
        }

        /// <inheritdoc />
        public List<KeyValuePair<object, FilterState>> FilterBy(List<string> entries)
        {
            var filterList = entries.Select(x => (object) x).ToList();
            return FilterBy(filterList);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Apply a filter to the collection.
        /// Also sets the MenuItem icon so that it is displayed correctly
        /// </summary>
        /// <param name="menuItem">MenuItem the filter is for</param>
        /// <param name="filter">The filter to apply</param>
        /// <returns>Indicate if filter was applied or remove</returns>
        private FilterState ApplyFilter(ToggleMenuFlyoutItem menuItem, string filter)
        {
            FilterState state;
            if (_selectedFilters.Contains(filter))
            {
                _selectedFilters.Remove(filter);
                ShapingEntry.CurrentFilterValues.Remove(filter);
                menuItem.IsChecked = false;
                state = FilterState.Removed;
            }
            else
            {
                _selectedFilters.Add(filter);
                ShapingEntry.CurrentFilterValues.Add(filter);
                menuItem.IsChecked = true;
                state = FilterState.Applied;
            }

            RefreshFiltering?.Invoke(this, EventArgs.Empty);
            return state;
        }

        /// <summary>
        /// Occurs when the user clicks the filter button
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void ButtonOnClick(object sender, RoutedEventArgs e)
        {
            var menu = GetContextMenu();
            menu.ShowAt(FlyoutButton, new FlyoutShowOptions
            {
                Placement = FlyoutPlacementMode.Bottom
            });
        }

        /// <summary>
        /// Occurs when the users clicks one of the filter entries
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event argument</param>
        /// <exception cref="NotImplementedException"></exception>
        private void EntryOnClick(object sender, RoutedEventArgs e)
        {
            var menuItem = (ToggleMenuFlyoutItem)sender;
            var header = menuItem.Text;
            ApplyFilter(menuItem, header);
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

            _menu = new();

            foreach (var filterValue in _filterValues)
            {
                var entry = new ToggleMenuFlyoutItem
                {
                    Text = filterValue
                };
                entry.Click += EntryOnClick;

                _menu.Items.Add(entry);
            }

            return _menu;
        }

        #endregion

        #region Variables

        /// <summary>
        /// Values that the collection can be filtered on
        /// </summary>
        private readonly List<string> _filterValues;

        /// <summary>
        /// List containing the filters that the user has selected
        /// </summary>
        private readonly List<string> _selectedFilters;

        /// <summary>
        /// The context menu containing the filter options
        /// </summary>
        private MenuFlyout? _menu;

        #endregion
    }
}
