using CommunityToolkit.WinUI.UI;
using VaraniumSharp.WinUI.FilterModule;
using VaraniumSharp.WinUI.GroupModule;
using VaraniumSharp.WinUI.SortModule;

namespace TestHelper.Sorting
{
    public class SortableEntry
    {
        #region Properties

        [SortableProperty("Accidental Sort", "Sort by accident")]
        public string AccidentalSort { get; set; }

        [SortableProperty("And another", "And another sort")]
        public string AndAnother { get; set; }

        [FilterableProperty("Bool filter", "Filter by my boolean value", FilterableType.Boolean, 0)]
        public bool BoolToFilter { get; set; }

        [FilterableProperty("Enum filter", "Filter values by enum", FilterableType.Enumeration, 1)]
        public SortableEnum EnumToFilter { get; init; }

        [SortableProperty("Even More", "Even more sorting")]
        public string EvenMore { get; set; }

        [GroupingProperty("Id", "Group by Id")]
        [SortableProperty("Id", "Sort by Id", IsDefault = true)]
        public int Id { get; init; }

        [SortableProperty("MoreSorting", "More sorting")]
        public string MoreSorting { get; set; }

        [SortableProperty("Position", "Sort by Position", DefaultSortDirection = SortDirection.Descending)]
        public int Position { get; init; }

        [FilterableProperty("Type", "Filter by type", FilterableType.PredefinedString, 2, "TestHelper.Sorting.PredefinedStrings, TestHelper", "PredefinedStringsCollection")]
        public string PredefinedType { get; set; }

        [FilterableProperty("Title", "Filter by Title", FilterableType.SearchableString, 3)]
        [GroupingProperty("Title", "Group by Title")]
        [SortableProperty("Title", "Sort by Title")]
        public string Title { get; init; }

        #endregion
    }
}