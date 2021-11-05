using System;
using CommunityToolkit.WinUI.UI;
using VaraniumSharp.WinUI.SortModule;

namespace VaraniumSharp.WinUI.Tests.SortModule
{
    public class SortableFixture
    {
        #region Properties

        [SortableProperty("DateSort", "Sort by date")]
        public DateTime DateSorValue { get; set; }

        public string NotSortable { get; set; }

        [SortableProperty("SortByMe", "Sort by me", DefaultSort = true, DefaultSortDirection = SortDirection.Descending)]
        public int SortByMe { get; set; }

        #endregion
    }

    public class SortableFixtureWithNestedEntry
    {
        public string NotSortable { get; set; }

        [SortableProperty(typeof(NestedFixture))]
        public NestedFixture Nested { get; set; }
    }

    public class NestedFixture
    {
        [SortableProperty("Nested", "Sort by nested property")]
        public int NestedSortProp { get; set; }

    }

}