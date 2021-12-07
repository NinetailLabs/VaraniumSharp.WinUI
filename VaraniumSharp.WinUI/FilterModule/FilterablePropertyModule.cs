using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.WinUI.Collections;
using VaraniumSharp.WinUI.ExtensionMethods;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.FilterModule
{
    /// <summary>
    /// Assist with filtering a <see cref="IAdvancedCollectionView"/> based on <see cref="FilterablePropertyAttribute"/>
    /// </summary>
    public sealed partial class FilterablePropertyModule : ShapingPropertyModuleBase<FilterablePropertyAttribute>
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="viewSource">
        /// Collection view to sort. Note the collection should have been constructed with isLiveShaping set to true.
        /// The <see cref="ExtendedAdvancedCollectionView"/> or <see cref="GroupingAdvancedCollectionView"/> is recommended.
        /// </param>
        public FilterablePropertyModule(IAdvancedCollectionView viewSource)
            : base(viewSource)
        {
            FilterControls = new List<UserControl>();
            _controlCreationDictionary = new();
            SetupDictionary();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Controls that are used for filtering
        /// </summary>
        public List<UserControl> FilterControls { get; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Fired when a control`s filter(s) has been updated and the ViewSource should be refreshed
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void ControlOnRefreshFiltering(object? sender, EventArgs e)
        {
            ViewSource.Refresh();
        }

        /// <inheritdoc />
        protected override ShapingEntry? CreateShapingEntry(string propertyName, ShapingPropertyAttributeBase attribute)
        {
            if (attribute is FilterablePropertyAttribute filterAttribute)
            {
                if (_controlCreationDictionary.ContainsKey(filterAttribute.FilterType))
                {
                    var action = _controlCreationDictionary[filterAttribute.FilterType];
                    action.Invoke(propertyName, filterAttribute, null); // TODO - Replace null with properties once we get that far
                }
                else
                {
                    throw new InvalidOperationException($"No control available to handle filter type {filterAttribute.FilterType}");
                }
            }

            if (_filterFunc != null)
            {
                ViewSource.Filter = new Predicate<object>(_filterFunc);
            }

            return null;
        }

        /// <inheritdoc />
        protected override void EntriesShapedByOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Checks if a filter already exists in the collection.
        /// The filter is only checked based on the display name
        /// </summary>
        /// <param name="filterDisplayName">Display name to check</param>
        /// <returns>True if the filter exists, otherwise false</returns>
        private bool FilterAlreadyExists(string filterDisplayName)
        {
            return FilterControls.Any(x => ((IFilterControl)x).FilterDisplayName == filterDisplayName);
        }

        /// <summary>
        /// Hook up required events and filters to a IFilterControl instance
        /// </summary>
        /// <param name="filterControl">The filter control to hook up</param>
        private void HookupFilterControl(IFilterControl filterControl)
        {
            filterControl.RefreshFiltering += ControlOnRefreshFiltering;
            if (filterControl is UserControl control)
            {
                FilterControls.Add(control);
                _filterFunc = _filterFunc == null
                    ? filterControl.Filter
                    : _filterFunc.AndAlso(filterControl.Filter);
            }
        }

        /// <summary>
        /// Set up the <see cref="_controlCreationDictionary"/> with the methods that can be used to create filter controls.
        /// </summary>
        private void SetupDictionary()
        {
            var methods = GetType()
                .GetRuntimeMethods()
                .Where(x => x.GetCustomAttributes().Any(z => z is FilterableControlCreationAttribute))
                .ToList();

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<FilterableControlCreationAttribute>();
                if (attribute != null)
                {
                    var executableFunc = (Action<string, FilterablePropertyAttribute, PropertyInfo>)Delegate.CreateDelegate(typeof(Action<string, FilterablePropertyAttribute, PropertyInfo>), this, method);
                    _controlCreationDictionary.Add(attribute.FilterType, executableFunc);
                }
            }
        }

        /// <inheritdoc />
        protected override void Shape(string propertyName)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Variables

        /// <summary>
        /// Dictionary containing Actions that can create and add the different controls for the <see cref="FilterableType"/>
        /// </summary>
        private readonly Dictionary<FilterableType, Action<string, FilterablePropertyAttribute, PropertyInfo>> _controlCreationDictionary;

        /// <summary>
        /// Filter function helper
        /// </summary>
        private Func<object, bool>? _filterFunc;

        #endregion
    }
}