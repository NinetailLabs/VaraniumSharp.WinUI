using CommunityToolkit.WinUI.UI;
using VaraniumSharp.WinUI.GroupModule;
using VaraniumSharp.WinUI.SortModule;

namespace TestHelper.Sorting
{
    public class SortableEntry
    {
        #region Properties

        [GroupingProperty("Id", "Group by Id")]
        [SortableProperty("Id", "Sort by Id", IsDefault = true)]
        public int Id { get; init; }

        [SortableProperty("Position", "Sort by Position", DefaultSortDirection = SortDirection.Descending)]
        public int Position { get; init; }

        [GroupingProperty("Title", "Group by Title")]
        [SortableProperty("Title", "Sort by Title")]
        public string Title { get; init; }

        [SortableProperty("Accidental Sort", "Sort by accident")]
        public string AccidentalSort { get; set; }

        [SortableProperty("MoreSorting", "More sorting")]
        public string MoreSorting { get; set; }

        [SortableProperty("Even More", "Even more sorting")]
        public string EvenMore { get; set; }

        [SortableProperty("And another", "And another sort")]
        public string AndAnother { get; set; }

        #endregion
    }
}