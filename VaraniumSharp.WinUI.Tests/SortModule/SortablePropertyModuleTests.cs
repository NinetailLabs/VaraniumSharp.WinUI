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
            sut.DisableDefaultShaping = true;
            sut.GenerateShapingEntries(typeof(SortableFixture));
            var act = new Action(() => sut.ShapeByMultipleProperties("DateSorValue", "IDoNotExist"));

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
            sut.GenerateShapingEntries(typeof(SortableFixture));
            var act = new Action(() => sut.RemoveShapingEntry("IDoNotExist"));

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
            sut.GenerateShapingEntries(typeof(SortableFixture));
            
            // act
            sut.ClearShapingOnClick();

            // assert
            fixture.AdvancedCollectionView.SortDescriptions.Count.Should().Be(0);
            sut.EntriesShapedBy.Count.Should().Be(0);
            sut.AvailableShapingEntries.Count.Should().Be(2);
        }

        [Fact]
        public void DisablingDefaultSortDuringGenerationDoesNotSort()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.DisableDefaultShaping = true;

            // act
            sut.GenerateShapingEntries(typeof(SortableFixture));

            // assert
            sut.AvailableShapingEntries.Count.Should().Be(2);
            fixture.AdvancedCollectionView.SortDescriptions.Count.Should().Be(0);
        }

        [Fact]
        public void GeneratingSortEntriesGeneratesExpectedEntries()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();
            
            var sut = fixture.GetInstance();

            // act
            sut.GenerateShapingEntries(typeof(SortableFixture));

            // assert
            sut.AvailableShapingEntries.Count.Should().Be(1);
            sut.AvailableShapingEntries.First().PropertyName.Should().Be("DateSorValue");
            sut.EntriesShapedBy.Count.Should().Be(1);
            sut.EntriesShapedBy.First().PropertyName.Should().Be("SortByMe");
        }

        [Fact]
        public void NestedPropertiesCanAlsoBePopulatedFromNestedPropertyList()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.NestedTypeList.Add(new []{ typeof(NestedFixture) });

            // act
            sut.GenerateShapingEntries(typeof(SortableFixtureWithNestedEntry));

            // assert
            sut.AvailableShapingEntries.Count.Should().Be(1);
            sut.AvailableShapingEntries.First().PropertyName.Should().Be("Nested.NestedSortProp");
        }

        [Fact]
        public void NestedPropertySortsAreCorrectlyGenerated()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();

            // act
            sut.GenerateShapingEntries(typeof(SortableFixtureWithNestedEntry));

            // assert
            sut.AvailableShapingEntries.Count.Should().Be(1);
            sut.AvailableShapingEntries.First().PropertyName.Should().Be("Nested.NestedSortProp");
        }

        [Fact]
        public void RearrangingTheCollectionCorrectlyMovesTheSortDescriptions()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.DisableDefaultShaping = true;
            sut.GenerateShapingEntries(typeof(SortableFixture));
            sut.ShapeByMultipleProperties("DateSorValue", "SortByMe");
            
            // act
            var entryToMove = sut.EntriesShapedBy.Last();
            sut.EntriesShapedBy.Remove(entryToMove);
            sut.EntriesShapedBy.Insert(0, entryToMove);

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
            sut.GenerateShapingEntries(typeof(SortableFixture));

            // act
            sut.RemoveShapingEntry("SortByMe");

            // assert
            sut.EntriesShapedBy.Count.Should().Be(0);
            fixture.AdvancedCollectionView.SortDescriptions.Count.Should().Be(0);
        }

        [Fact]
        public void RemovingASortingEntryNotSortedOnRemovesItFromAvailableSort()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateShapingEntries(typeof(SortableFixture));

            // act
            sut.RemoveShapingEntry("DateSorValue");

            // assert
            sut.AvailableShapingEntries.Count.Should().Be(0);
        }

        [Fact]
        public void RequestingASortProgramaticallyClearsTheExistingSort()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateShapingEntries(typeof(SortableFixture));

            // act
            sut.RequestShaping("DateSorValue");

            // assert
            sut.EntriesShapedBy.Count.Should().Be(1);
            sut.EntriesShapedBy.First().PropertyName.Should().Be("DateSorValue");
            sut.AvailableShapingEntries.Count.Should().Be(1);
            sut.AvailableShapingEntries.First().PropertyName.Should().Be("SortByMe");
            fixture.AdvancedCollectionView.SortDescriptions.Count.Should().Be(1);
            fixture.AdvancedCollectionView.SortDescriptions.First().PropertyName.Should().Be("DateSorValue");
        }

        [Fact]
        public void RequestingMoveOfAvailableEntryToSortMovesTheEntry()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateShapingEntries(typeof(SortableFixture));
            sut.SelectedAvailableEntry = sut.AvailableShapingEntries.First();

            // act
            sut.MoveEntryFromAvailableToShapedBy();

            // assert
            sut.AvailableShapingEntries.Count.Should().Be(0);
            sut.EntriesShapedBy.Count.Should().Be(2);
        }

        [Fact]
        public void RequestingMoveOfSortedEntryMovesItToAvailableEntries()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateShapingEntries(typeof(SortableFixture));
            sut.SelectedShapedByEntry = sut.EntriesShapedBy.First();

            // act
            sut.MoveEntryFromShapedByToAvailable();

            // assert
            sut.EntriesShapedBy.Count.Should().Be(0);
            sut.AvailableShapingEntries.Count.Should().Be(2);
        }

        [Fact]
        public void RequestingSortByPropertyNotInTheCollectionThrowsAnException()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateShapingEntries(typeof(SortableFixture));
            var act = new Action(() => sut.RequestShaping("IDoNotExist"));

            // act
            // assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void SelectingAnAvailableEntryEnablesItForMove()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateShapingEntries(typeof(SortableFixture));

            // act
            sut.SelectedAvailableEntry = sut.AvailableShapingEntries.First();

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
            sut.GenerateShapingEntries(typeof(SortableFixture));

            // act
            sut.SelectedShapedByEntry = sut.EntriesShapedBy.First();

            // assert
            sut.SelectedShapedByEntry.Should().NotBeNull();
            sut.MoveShapedByEnabled.Should().BeTrue();
        }

        [Fact]
        public void SortingCollectionByMultipleEntriesCorrectlySortsTheCollection()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.DisableDefaultShaping = true;
            sut.GenerateShapingEntries(typeof(SortableFixture));

            // act
            sut.ShapeByMultipleProperties("DateSorValue", "SortByMe");

            // assert
            sut.AvailableShapingEntries.Count.Should().Be(0);
            sut.EntriesShapedBy.Count.Should().Be(2);
            sut.EntriesShapedBy.First().PropertyName.Should().Be("DateSorValue");
            sut.EntriesShapedBy.Last().PropertyName.Should().Be("SortByMe");
            fixture.AdvancedCollectionView.SortDescriptions.Count.Should().Be(2);
            fixture.AdvancedCollectionView.SortDescriptions.First().PropertyName.Should().Be("DateSorValue");
            fixture.AdvancedCollectionView.SortDescriptions.Last().PropertyName.Should().Be("SortByMe");
        }

        [Fact]
        public void SortOrderEntryIsCorrectlyPopulated()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();

            // act
            sut.GenerateShapingEntries(typeof(SortableFixture));

            // assert
            var entry = sut.AvailableShapingEntries.First() as SortableShapingEntry;
            entry.Should().NotBeNull();
            entry?.PropertyName.Should().Be("DateSorValue");
            entry?.SortDirection.Should().Be(SortDirection.Ascending);
            entry?.Header.Should().Be("DateSort");
            entry?.Tooltip.Should().Be("Sort by date");
        }

        [Fact]
        public void WhenASortedEntriesSortDirectionChangesTheSortIsUpdated()
        {
            // arrange
            var fixture = new SortablePropertyModuleFixture();

            var sut = fixture.GetInstance();
            sut.GenerateShapingEntries(typeof(SortableFixture));

            // act
            (sut.EntriesShapedBy.First() as SortableShapingEntry)?.ChangeDirectionClick();

            // assert
            (sut.EntriesShapedBy.First() as SortableShapingEntry)?.Should().NotBeNull();
            (sut.EntriesShapedBy.First() as SortableShapingEntry)?.SortDirection.Should().Be(SortDirection.Ascending);
            (sut.EntriesShapedBy.First() as SortableShapingEntry)?.DirectionIcon.Should().Be("Asc");
            fixture.AdvancedCollectionView.SortDescriptions.First().Direction.Should().Be(SortDirection.Ascending);
        }
    }
}