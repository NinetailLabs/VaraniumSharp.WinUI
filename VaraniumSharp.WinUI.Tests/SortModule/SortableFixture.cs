using System;
using CommunityToolkit.WinUI.UI;
using VaraniumSharp.WinUI.SortModule;

namespace VaraniumSharp.WinUI.Tests.SortModule
{
    public class SortableFixture
    {
        public SortableFixture()
        {
            NotSortable = string.Empty;
        }

        #region Properties

        [SortableProperty("DateSort", "Sort by date")]
        public DateTime DateSorValue { get; set; }

        public string NotSortable { get; set; }

        [SortableProperty("SortByMe", "Sort by me", IsDefault = true, DefaultSortDirection = SortDirection.Descending)]
        public int SortByMe { get; set; }

        #endregion
    }

    public class SortableFixtureWithNestedEntry
    {
        public SortableFixtureWithNestedEntry()
        {
            NotSortable = string.Empty;
            Nested = new NestedFixture();
        }

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