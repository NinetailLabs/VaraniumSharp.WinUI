using System.ComponentModel;
using CommunityToolkit.WinUI.UI;
using VaraniumSharp.WinUI.FilterModule;
using VaraniumSharp.WinUI.GroupModule;
using VaraniumSharp.WinUI.SortModule;

namespace TestHelper.Sorting
{
    public class SortableEntry : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Properties

        [SortableProperty("Accidental Sort", "Sort by accident")]
        public string AccidentalSort { get; set; }

        [SortableProperty("And another", "And another sort")]
        public string AndAnother { get; set; }

        [FilterableProperty("Long Entry Title", "Filter by long entry title", FilterableType.SearchableString, 4)]
        public string Another { get; init; }

        [FilterableProperty("Bool filter", "Filter by my boolean value", FilterableType.Boolean, 0)]
        public bool BoolToFilter { get; set; }

        [SortableProperty(typeof(EmbeddedEntry))]
        public EmbeddedEntry EmbeddedEntry
        {
            get => _embeddedEntry;
            set
            {
                _embeddedEntry = value;
                // Need to forward the request in order for sorting to work correctly
                _embeddedEntry.PropertyChanged += (sender, args) =>
                {
                    PropertyChanged?.Invoke(this,
                        new($"{nameof(EmbeddedEntry)}.{args.PropertyName}"));
                };
            }
        }

        /// <summary>
        /// Backing variable for the <see cref="EmbeddedEntry"/> property
        /// </summary>
        private EmbeddedEntry _embeddedEntry;

        [FilterableProperty("Enum filter", "Filter values by enum", FilterableType.Enumeration, 1)]
        public SortableEnum EnumToFilter { get; init; }

        [SortableProperty("Even More", "Even more sorting")]
        public string EvenMore { get; set; }

        [GroupingProperty("Id", "Group by Id")]
        public int Id { get; set; }

        [SortableProperty("MoreSorting", "More sorting")]
        public string MoreSorting { get; set; }

        [SortableProperty("Position", "Sort by Position", DefaultSortDirection = SortDirection.Descending)]
        public int Position { get; init; }

        [FilterableProperty("Type", "Filter by type", FilterableType.PredefinedString, 2, "TestHelper.Sorting.PredefinedStrings, TestHelper", "PredefinedStringsCollection")]
        public string PredefinedType { get; set; }

        [FilterableProperty("Title", "Filter by Title", FilterableType.SearchableString, 3)]
        [GroupingProperty("Title", "Group by Title")]
        [SortableProperty("Title", "Sort by Title")]
        public string Title { get; set; }

        /// <summary>
        /// A random value that isn't used for sorting or grouping
        /// </summary>
        public bool RandomValue { get; set; }

        #endregion
    }
}