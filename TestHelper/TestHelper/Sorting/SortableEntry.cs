using CommunityToolkit.WinUI.UI;
using VaraniumSharp.WinUI.SortModule;

namespace TestHelper.Sorting
{
    public class SortableEntry
    {
        #region Properties

        [SortableProperty("Id", "Sort by Id", DefaultSort = true)]
        public int Id { get; init; }

        [SortableProperty("Position", "Sort by Position", DefaultSortDirection = SortDirection.Descending)]
        public int Position { get; init; }

        [SortableProperty("Title", "Sort by Title")]
        public string Title { get; init; }

        [SortableProperty("Accidental Sort", "Sort by accident")]
        public string AccidentalSort { get; set; }

        #endregion
    }
}