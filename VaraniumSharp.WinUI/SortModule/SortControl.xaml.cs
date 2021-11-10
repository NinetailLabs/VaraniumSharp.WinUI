using System.ComponentModel;
using VaraniumSharp.WinUI.DragAndDrop;
using Windows.ApplicationModel.DataTransfer;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Control that provides drag and drop sorting
    /// </summary>
    public sealed partial class SortControl : INotifyPropertyChanged
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SortControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Module that contains the sortable properties
        /// </summary>
        public SortablePropertyModule? SortablePropertyModule 
        {
            get => _sortablePropertyModule;
            set
            {
                _sortablePropertyModule = value;
                if (value != null)
                {
                    SortDragModule = new("SortEntry", DataPackageOperation.Move, value.AvailableSortEntries, value.EntriesSortedBy);
                    AvailableDragModule = new("SortEntry", DataPackageOperation.Move, value.EntriesSortedBy, value.AvailableSortEntries);
                }
                else
                {
                    SortDragModule = null;
                    AvailableDragModule = null;
                }
            }
        }

        /// <summary>
        /// DragModule used for drag and drop from the <see cref="SortablePropertyModule.AvailableSortEntries"/> to the <see cref="SortablePropertyModule.EntriesSortedBy"/>
        /// </summary>
        public DragModule<SortOrderEntry>? SortDragModule { get; private set; }

        /// <summary>
        /// DragModule used for drag and drop from the <see cref="SortablePropertyModule.EntriesSortedBy"/> to the <see cref="SortablePropertyModule.AvailableSortEntries"/>
        /// </summary>
        public DragModule<SortOrderEntry>? AvailableDragModule { get; private set; }

        /// <inheritdoc />
#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        /// <summary>
        /// Backing variable for the <see cref="SortablePropertyModule"/> property
        /// </summary>
        private SortablePropertyModule? _sortablePropertyModule;
    }
}
