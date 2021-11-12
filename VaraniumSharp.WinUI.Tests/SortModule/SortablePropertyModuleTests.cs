using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommunityToolkit.WinUI.UI;
using FluentAssertions;
using VaraniumSharp.WinUI.SortModule;
using Xunit;

namespace VaraniumSharp.WinUI.Tests.SortModule
{
    public class SortablePropertyModuleTests
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local", Justification = "Test Fixture - Unit tests require access to Mocks")]
        private class SortablePropertyModuleFixture
        {
            #region Properties

            public AdvancedCollectionView AdvancedCollectionView { get; } = new();

            #endregion

            #region Public Methods

            public SortablePropertyModule GetInstance()
            {
                return new SortablePropertyModule(AdvancedCollectionView);
            }

            #endregion
        }

        [Fact]
        public void AttemptingToMultiSortCollectionWithAnInvalidPropertyThrowsAnException()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.DisableDefaultSort = true;
            sut.GenerateSortEntries(typeof(SortableFixture));
            var act = new Action(() => sut.SortByMultipleProperties("DateSorValue", "IDoNotExist"));

            // act
            // assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void AttemptingToRemoveAnEntryThatDoesNotExistThrowsAnException()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateSortEntries(typeof(SortableFixture));
            var act = new Action(() => sut.RemoveSortEntry("IDoNotExist"));

            // act
            // assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ClearingSortRemovesAllSortDescriptionsFromTheSource()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateSortEntries(typeof(SortableFixture));
            
            // act
            sut.ClearSortOnClick();

            // assert
            fixture.AdvancedCollectionView.SortDescriptions.Count.Should().Be(0);
            sut.EntriesSortedBy.Count.Should().Be(0);
            sut.AvailableSortEntries.Count.Should().Be(2);
        }

        [Fact]
        public void DisablingDefaultSortDuringGenerationDoesNotSort()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.DisableDefaultSort = true;

            // act
            sut.GenerateSortEntries(typeof(SortableFixture));

            // assert
            sut.AvailableSortEntries.Count.Should().Be(2);
            fixture.AdvancedCollectionView.SortDescriptions.Count.Should().Be(0);
        }

        [Fact]
        public void GeneratingSortEntriesGeneratesExpectedEntries()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();
            
            var sut = fixture.GetInstance();

            // act
            sut.GenerateSortEntries(typeof(SortableFixture));

            // assert
            sut.AvailableSortEntries.Count.Should().Be(1);
            sut.AvailableSortEntries.First().PropertyName.Should().Be("DateSorValue");
            sut.EntriesSortedBy.Count.Should().Be(1);
            sut.EntriesSortedBy.First().PropertyName.Should().Be("SortByMe");
        }

        [Fact]
        public void NestedPropertiesCanAlsoBePopulatedFromNestedPropertyList()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.NestedTypeList.Add(new []{ typeof(NestedFixture) });

            // act
            sut.GenerateSortEntries(typeof(SortableFixtureWithNestedEntry));

            // assert
            sut.AvailableSortEntries.Count.Should().Be(1);
            sut.AvailableSortEntries.First().PropertyName.Should().Be("Nested.NestedSortProp");
        }

        [Fact]
        public void NestedPropertySortsAreCorrectlyGenerated()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();

            // act
            sut.GenerateSortEntries(typeof(SortableFixtureWithNestedEntry));

            // assert
            sut.AvailableSortEntries.Count.Should().Be(1);
            sut.AvailableSortEntries.First().PropertyName.Should().Be("Nested.NestedSortProp");
        }

        [Fact]
        public void RearrangingTheCollectionCorrectlyMovesTheSortDescriptions()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.DisableDefaultSort = true;
            sut.GenerateSortEntries(typeof(SortableFixture));
            sut.SortByMultipleProperties("DateSorValue", "SortByMe");
            
            // act
            var entryToMove = sut.EntriesSortedBy.Last();
            sut.EntriesSortedBy.Remove(entryToMove);
            sut.EntriesSortedBy.Insert(0, entryToMove);

            // assert
            fixture.AdvancedCollectionView.SortDescriptions.Count.Should().Be(2);
            fixture.AdvancedCollectionView.SortDescriptions.First().PropertyName.Should().Be("SortByMe");
            fixture.AdvancedCollectionView.SortDescriptions.Last().PropertyName.Should().Be("DateSorValue");
        }

        [Fact]
        public void RemovingASortingEntryCurrentlySortedOnRemovesTheSort()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateSortEntries(typeof(SortableFixture));

            // act
            sut.RemoveSortEntry("SortByMe");

            // assert
            sut.EntriesSortedBy.Count.Should().Be(0);
            fixture.AdvancedCollectionView.SortDescriptions.Count.Should().Be(0);
        }

        [Fact]
        public void RemovingASortingEntryNotSortedOnRemovesItFromAvailableSort()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateSortEntries(typeof(SortableFixture));

            // act
            sut.RemoveSortEntry("DateSorValue");

            // assert
            sut.AvailableSortEntries.Count.Should().Be(0);
        }

        [Fact]
        public void RequestingASortProgramaticallyClearsTheExistingSort()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateSortEntries(typeof(SortableFixture));

            // act
            sut.RequestSort("DateSorValue");

            // assert
            sut.EntriesSortedBy.Count.Should().Be(1);
            sut.EntriesSortedBy.First().PropertyName.Should().Be("DateSorValue");
            sut.AvailableSortEntries.Count.Should().Be(1);
            sut.AvailableSortEntries.First().PropertyName.Should().Be("SortByMe");
            fixture.AdvancedCollectionView.SortDescriptions.Count.Should().Be(1);
            fixture.AdvancedCollectionView.SortDescriptions.First().PropertyName.Should().Be("DateSorValue");
        }

        [Fact]
        public void RequestingSortByPropertyNotInTheCollectionThrowsAnException()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateSortEntries(typeof(SortableFixture));
            var act = new Action(() => sut.RequestSort("IDoNotExist"));

            // act
            // assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void SortingCollectionByMultipleEntriesCorrectlySortsTheCollection()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.DisableDefaultSort = true;
            sut.GenerateSortEntries(typeof(SortableFixture));

            // act
            sut.SortByMultipleProperties("DateSorValue", "SortByMe");

            // assert
            sut.AvailableSortEntries.Count.Should().Be(0);
            sut.EntriesSortedBy.Count.Should().Be(2);
            sut.EntriesSortedBy.First().PropertyName.Should().Be("DateSorValue");
            sut.EntriesSortedBy.Last().PropertyName.Should().Be("SortByMe");
            fixture.AdvancedCollectionView.SortDescriptions.Count.Should().Be(2);
            fixture.AdvancedCollectionView.SortDescriptions.First().PropertyName.Should().Be("DateSorValue");
            fixture.AdvancedCollectionView.SortDescriptions.Last().PropertyName.Should().Be("SortByMe");
        }

        [Fact]
        public void WhenASortedEntriesSortDirectionChangesTheSortIsUpdated()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateSortEntries(typeof(SortableFixture));

            // act
            sut.EntriesSortedBy.First().ChangeDirectionClick();
            
            // assert
            sut.EntriesSortedBy.First().SortDirection.Should().Be(SortDirection.Ascending);
            sut.EntriesSortedBy.First().SortIcon.Should().Be("Asc");
            fixture.AdvancedCollectionView.SortDescriptions.First().Direction.Should().Be(SortDirection.Ascending);
        }

        [Fact]
        public void SortOrderEntryIsCorrectlyPopulated()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();

            // act
            sut.GenerateSortEntries(typeof(SortableFixture));

            // assert
            var entry = sut.AvailableSortEntries.First();
            entry.PropertyName.Should().Be("DateSorValue");
            entry.SortDirection.Should().Be(SortDirection.Ascending);
            entry.SortHeader.Should().Be("DateSort");
            entry.SortTooltip.Should().Be("Sort by date");
        }

        [Fact]
        public void SelectingAnAvailableEntryEnablesItForMove()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateSortEntries(typeof(SortableFixture));

            // act
            sut.SelectedAvailableEntry = sut.AvailableSortEntries.First();

            // assert
            sut.SelectedAvailableEntry.Should().NotBeNull();
            sut.MoveAvailableEnabled.Should().BeTrue();
        }

        [Fact]
        public void SelectingSortByEntryEnablesItForMove()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateSortEntries(typeof(SortableFixture));

            // act
            sut.SelectedSortByEntry = sut.EntriesSortedBy.First();

            // assert
            sut.SelectedSortByEntry.Should().NotBeNull();
            sut.MoveSortedByEnabled.Should().BeTrue();
        }

        [Fact]
        public void RequestingMoveOfAvailableEntryToSortMovesTheEntry()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateSortEntries(typeof(SortableFixture));
            sut.SelectedAvailableEntry = sut.AvailableSortEntries.First();

            // act
            sut.MoveEntryFromAvailableToSortedBy();

            // assert
            sut.AvailableSortEntries.Count.Should().Be(0);
            sut.EntriesSortedBy.Count.Should().Be(2);
        }

        [Fact]
        public void RequestingMoveOfSortedEntryMovesItToAvailableEntries()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateSortEntries(typeof(SortableFixture));
            sut.SelectedSortByEntry = sut.EntriesSortedBy.First();

            // act
            sut.MoveEntryFromSortedByToAvailable();

            // assert
            sut.EntriesSortedBy.Count.Should().Be(0);
            sut.AvailableSortEntries.Count.Should().Be(2);
        }
    }
}