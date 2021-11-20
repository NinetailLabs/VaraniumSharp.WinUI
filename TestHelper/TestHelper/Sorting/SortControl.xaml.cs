using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.Collections;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.GroupModule;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;
using VaraniumSharp.WinUI.SortModule;

namespace TestHelper.Sorting
{
    [AutomaticContainerRegistration(typeof(SortControl))]
    [DisplayComponent("Sort Control", "84dc893c-f7f0-4e74-9922-b059c0d234b7", "Sorting", 100, 100, typeof(SortControl))]
    public sealed partial class SortControl: ISortableDisplayComponent
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

            GroupingPropertyModule = new GroupingPropertyModule(CollectionView, SortableGrid)
            {
                DisableDefaultShaping = true
            };
            GroupingPropertyModule.GenerateShapingEntries(typeof(SortableEntry));
            GroupingPropertyModule.ShapeByMultipleProperties("Title");

            SetupCollection();
        }

        #endregion

        #region Events

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

        public GroupingPropertyModule GroupingPropertyModule { get; }
        public Guid InstanceId { get; set; }

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

        private object Group(object arg)
        {
            var obj = arg as SortableEntry;
            return $"Group: {obj?.Id ?? 0}";
        }

        private void SetupCollection()
        {
            Entries.Add(new SortableEntry
            {
                Id = 2,
                Title = "Q",
                Position = 2
            });

            Entries.Add(new SortableEntry
            {
                Id = 1,
                Title = "Z",
                Position = 3
            });

            Entries.Add(new SortableEntry
            {
                Id = 3,
                Title = "H",
                Position = 1
            });

            Entries.Add(new SortableEntry
            {
                Id = 3,
                Title = "A",
                Position = 1
            });

            Entries.Add(new SortableEntry
            {
                Id = 4,
                Title = "A",
                Position = 3
            });
        }

        /// <summary>
        /// Occurs when the <see cref="SortablePropertyModule"/> sort has changed
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void SortablePropertyModuleShapingChanged(object sender, EventArgs e)
        {
            SortChanged?.Invoke(this, e);
        }

        #endregion
    }
}
