using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using VaraniumSharp.WinUI.ExtensionMethods;

namespace VaraniumSharp.WinUI.FilterModule.Controls
{
    /// <summary>
    /// Control used to filter based on an enum value
    /// </summary>
    public sealed partial class DropDownEnumFilter : IFilterControl, INotifyPropertyChanged
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="filterShapingEntry">Entry that contains the details about the filter</param>
        /// <param name="filterValues">Values that should be filtered by</param>
        public DropDownEnumFilter(FilterShapingEntry filterShapingEntry, List<object> filterValues)
        {
            InitializeComponent();
            ShapingEntry = filterShapingEntry;
            _filterValues = filterValues;
            _selectedFilters = new();
            _enumEntries = new();
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
            return property is Enum enumToFilter
                           && _selectedFilters.Contains(enumToFilter);
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
                    var filterValue = entry.ToString();
                    var menuItem = _menu!.Items
                        .Select(x => (ToggleMenuFlyoutItem) x)
                        .First(x => x.Text.Equals(filterValue, StringComparison.InvariantCultureIgnoreCase));
                    if (!_filterValues.Contains(entry) || menuItem == null)
                    {
                        response.Add(new KeyValuePair<object, FilterState>(entries, FilterState.NotFound));
                        continue;
                    }

                    var result = ApplyFilter(menuItem, entry);
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
            var resultList = new List<object>();
            foreach (var entry in entries)
            {
                var toAdd = _filterValues.FirstOrDefault(x => x.ToString() == entry);
                if (toAdd != null)
                {
                    resultList.Add(toAdd);
                }
            }

            return FilterBy(resultList);
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
        private FilterState ApplyFilter(ToggleMenuFlyoutItem menuItem, object filter)
        {
            FilterState state;
            if (_selectedFilters.Contains(filter))
            {
                _selectedFilters.Remove(filter);
                var filterString = filter.ToString();
                if (filterString != null)
                {
                    ShapingEntry.CurrentFilterValues.Remove(filterString);
                }
                menuItem.IsChecked = false;
                state = FilterState.Removed;
            }
            else
            {
                _selectedFilters.Add(filter);
                var filterString = filter.ToString();
                if (filterString != null)
                {
                    ShapingEntry.CurrentFilterValues.Add(filterString);
                }
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
            var entry = _enumEntries[header];
            ApplyFilter(menuItem, entry);
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
                var type = filterValue.GetType();
                var memInfo = type.GetMember(filterValue.ToString() ?? string.Empty);
                var displayAttribute = (DisplayAttribute?)memInfo.FirstOrDefault()?.GetCustomAttributes(typeof(DisplayAttribute)).FirstOrDefault();
                var headerText = displayAttribute?.Name ?? filterValue.ToString();
                _enumEntries.TryAdd(headerText!, filterValue);

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
        /// Dictionary containing the Enum entries and their displayed headers
        /// </summary>
        private readonly Dictionary<string, object> _enumEntries;

        /// <summary>
        /// Values that the collection can be filtered on
        /// </summary>
        private readonly List<object> _filterValues;

        /// <summary>
        /// List containing the filters that the user has selected
        /// </summary>
        private readonly List<object> _selectedFilters;

        /// <summary>
        /// The context menu containing the filter options
        /// </summary>
        private MenuFlyout? _menu;

        #endregion
    }
}
