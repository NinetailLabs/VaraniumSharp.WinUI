using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.Collections;
using VaraniumSharp.WinUI.CustomPaneBase;
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
            CollectionView = new ExtendedAdvancedCollectionView(Entries, true);
            SortablePropertyModule = new SortablePropertyModule(CollectionView);
            SortablePropertyModule.DisableDefaultSort = true;
            SortablePropertyModule.SortChanged += SortablePropertyModule_SortChanged;
            SortablePropertyModule.GenerateSortEntries(typeof(SortableEntry));
            SortablePropertyModule.RemoveSortEntry("AccidentalSort");
            SetupCollection();
        }

        /// <summary>
        /// Occurs when the <see cref="SortablePropertyModule"/> sort has changed
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void SortablePropertyModule_SortChanged(object sender, EventArgs e)
        {
            SortChanged?.Invoke(this, e);
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

        public ExtendedAdvancedCollectionView CollectionView { get; }

        public Guid ContentId => Guid.Parse("84dc893c-f7f0-4e74-9922-b059c0d234b7");

        private ObservableCollection<SortableEntry> Entries { get; }

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

        public Task SetControlSizeAsync(double width, double height)
        {
            Width = width;
            Height = height;
            return Task.CompletedTask;
        }

        #endregion

        #region Private Methods

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
        }

        /// <inheritdoc/>
        public void InitSortOrder(List<SortEntryStorageModel> sortEntries)
        {
            foreach(var entry in sortEntries)
            {
                var availableEntry = SortablePropertyModule.AvailableSortEntries.FirstOrDefault(x => x.PropertyName == entry.PropertyName);
                if(availableEntry != null)
                {
                    availableEntry.SortDirection = entry.SortDirection;
                }
            }

            SortablePropertyModule.SortByMultipleProperties(sortEntries.Select(x => x.PropertyName).ToArray());
        }

        #endregion
    }
}
