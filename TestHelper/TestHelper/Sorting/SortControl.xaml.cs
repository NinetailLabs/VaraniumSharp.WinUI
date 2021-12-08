using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.Collections;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.FilterModule;
using VaraniumSharp.WinUI.GroupModule;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;
using VaraniumSharp.WinUI.SortModule;

namespace TestHelper.Sorting
{
    [AutomaticContainerRegistration(typeof(SortControl))]
    [DisplayComponent("Sort Control", "84dc893c-f7f0-4e74-9922-b059c0d234b7", "Sorting", 100, 100, typeof(SortControl))]
    public sealed partial class SortControl: ISortableDisplayComponent, IGroupingDisplayComponent, IFilteringDisplayComponent
    {
        #region Constructor

        public SortControl()
        {
            InitializeComponent();
            Entries = new();
            CollectionView = new GroupingAdvancedCollectionView(Entries);
            SortablePropertyModule = new SortablePropertyModule(CollectionView)
            {
                DisableDefaultShaping = true
            };
            SortablePropertyModule.ShapingChanged += SortablePropertyModuleShapingChanged;
            SortablePropertyModule.GenerateShapingEntries(typeof(SortableEntry));
            SortablePropertyModule.RemoveShapingEntry("AccidentalSort");

            GroupingPropertyModule = new(CollectionView, SortableGrid)
            {
                DisableDefaultShaping = true
            };
            GroupingPropertyModule.ShapingChanged += GroupingPropertyModuleOnShapingChanged;
            GroupingPropertyModule.GenerateShapingEntries(typeof(SortableEntry));

            FilterablePropertyModule = new FilterablePropertyModule(CollectionView);
            FilterablePropertyModule.ShapingChanged += FilterablePropertyModuleOnShapingChanged;
            FilterablePropertyModule.GenerateShapingEntries(typeof(SortableEntry));

            SetupCollection();
        }

        #endregion

        #region Events

        public event EventHandler? FilterChanged;

        public event EventHandler? GroupChanged;

        /// <inheritdoc/>
#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        public event EventHandler? SortChanged;

        #endregion

        #region Properties

        public GroupingAdvancedCollectionView CollectionView { get; }

        public Guid ContentId => Guid.Parse("84dc893c-f7f0-4e74-9922-b059c0d234b7");

        private ObservableCollection<SortableEntry> Entries { get; }

        public FilterablePropertyModule FilterablePropertyModule { get; }
        public GroupingPropertyModule GroupingPropertyModule { get; }

        public Guid InstanceId { get; set; }

        public SortableEntry? SelectedEntry { get; set; }

        public bool ShowResizeHandle { get; set; }

        public SortablePropertyModule SortablePropertyModule { get; }

        public bool StartupLoad { get; set; }
        public string Title { get; set; } = "Sort Control";

        #endregion

        #region Public Methods

        public Task InitAsync()
        {
            // Not used for now
            return Task.CompletedTask;
        }

        public void InitFilterOrder(List<FilterEntryStorageModel> filterEntries)
        {
            FilterablePropertyModule.ApplyFilters(filterEntries);
        }

        public void InitGroupOrder(List<GroupEntryStorageModel> groupEntries)
        {
            GroupingPropertyModule.ShapeByMultipleProperties(groupEntries.Select(x => x.PropertyName).ToArray());
        }

        /// <inheritdoc/>
        public void InitSortOrder(List<SortEntryStorageModel> sortEntries)
        {
            foreach(var entry in sortEntries)
            {
                if(SortablePropertyModule.AvailableShapingEntries.FirstOrDefault(x => x.PropertyName == entry.PropertyName) is SortableShapingEntry availableEntry)
                {
                    availableEntry.SortDirection = entry.SortDirection;
                }
            }

            SortablePropertyModule.ShapeByMultipleProperties(sortEntries.Select(x => x.PropertyName).ToArray());
        }

        public Task SetControlSizeAsync(double width, double height)
        {
            Width = width;
            Height = height;
            return Task.CompletedTask;
        }

        #endregion

        #region Private Methods

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var rnd = new Random();
            Entries.Add(new SortableEntry
            {
                Id = rnd.Next(100),
                Title = "B",
                Position = 4
            });
        }

        private void ButtonDeleteAndReadd(object sender, RoutedEventArgs e)
        {
            Entries.Clear();
            SetupCollection();
        }

        private void ButtonDeleteOnClick(object sender, RoutedEventArgs e)
        {
            if (SelectedEntry != null)
            {
                Entries.Remove(SelectedEntry);
            }
        }

        private void FilterablePropertyModuleOnShapingChanged(object? sender, EventArgs e)
        {
            FilterChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when the <see cref="GroupingPropertyModule"/> groups has changed
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void GroupingPropertyModuleOnShapingChanged(object? sender, EventArgs e)
        {
            GroupChanged?.Invoke(this, e);
        }

        private void SetupCollection()
        {
            Entries.Add(new SortableEntry
            {
                Id = 2,
                Title = "Q",
                Position = 2,
                BoolToFilter = true
            });

            Entries.Add(new SortableEntry
            {
                Id = 1,
                Title = "T",
                Position = 3,
                BoolToFilter = false
            });

            Entries.Add(new SortableEntry
            {
                Id = 3,
                Title = "H",
                Position = 1,
                BoolToFilter = true
            });

            Entries.Add(new SortableEntry
            {
                Id = 3,
                Title = "A",
                Position = 1,
                BoolToFilter = false
            });

            Entries.Add(new SortableEntry
            {
                Id = 4,
                Title = "A",
                Position = 3,
                BoolToFilter = false
            });
        }

        /// <summary>
        /// Occurs when the <see cref="SortablePropertyModule"/> sort has changed
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void SortablePropertyModuleShapingChanged(object? sender, EventArgs e)
        {
            SortChanged?.Invoke(this, e);
        }

        #endregion
    }
}
