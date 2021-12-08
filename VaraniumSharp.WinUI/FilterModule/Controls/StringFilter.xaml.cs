using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using VaraniumSharp.WinUI.ExtensionMethods;

namespace VaraniumSharp.WinUI.FilterModule.Controls
{

    /// <summary>
    /// Control used to filter based on a string
    /// </summary>
    public sealed partial class StringFilter : IFilterControl, INotifyPropertyChanged
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="filterShapingEntry">Entry that contains details about the filter</param>
        public StringFilter(FilterShapingEntry filterShapingEntry)
        {
            InitializeComponent();
            ShapingEntry = filterShapingEntry;
            _filterString = string.Empty;
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

        /// <summary>
        /// The string used to filter on
        /// </summary>
        public string FilterString
        {
            get => _filterString;
            set
            {
                _filterString = value;
                RefreshFiltering?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public FilterShapingEntry ShapingEntry { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public bool Filter(object obj)
        {
            if (string.IsNullOrEmpty(FilterString))
            {
                return true;
            }

            var property = obj.GetNestedPropertyValue(ShapingEntry.PropertyName);

            return property is string stringToFilter
                   && stringToFilter.ToLowerInvariant().Contains(FilterString.ToLowerInvariant());
        }

        /// <inheritdoc />
        public List<KeyValuePair<object, FilterState>> FilterBy(List<object> entries)
        {
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
                    var entryString = entry.ToString();
                    if (string.IsNullOrEmpty(entryString))
                    {
                        response.Add(new KeyValuePair<object, FilterState>(entry, FilterState.NotValid));
                        continue;
                    }

                    FilterString = entryString;
                    response.Add(new KeyValuePair<object, FilterState>(entry, FilterState.Applied));
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
            var resultList = entries
                .Select(x => (object) x)
                .ToList();

            return FilterBy(resultList);
        }

        #endregion

        #region Variables

        /// <summary>
        /// Backing variable for the <see cref="FilterString"/> property
        /// </summary>
        private string _filterString;

        #endregion
    }
}
