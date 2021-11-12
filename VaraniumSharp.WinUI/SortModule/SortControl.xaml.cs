using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
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
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SortControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        /// <inheritdoc />
#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        #endregion

        #region Properties

        /// <summary>
        /// DragModule used for drag and drop from the sorted by entries to the available entries
        /// </summary>
        public DragModule<SortOrderEntry>? AvailableDragModule { get; private set; }

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
        /// DragModule used for drag and drop from the available entries to the sorted by entries
        /// </summary>
        public DragModule<SortOrderEntry>? SortDragModule { get; private set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Change the direction in which the selected item is sorted
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        private void FlipSortDirection(object sender)
        {
            if (sender is GridView { Name: "AvailableGrid" })
            {
                SortablePropertyModule?.SelectedAvailableEntry?.ChangeDirectionClick();
            }
            else if (sender is GridView { Name: "SortGrid" })
            {
                SortablePropertyModule?.SelectedSortByEntry?.ChangeDirectionClick();
            }
        }

        /// <summary>
        /// Occurs when a key is pressed while the user is in either of the sort grids
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridView_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                HandleSelectedItemMove(sender);
                return;
            }

            if(e.Key.ToString() == "221" || e.Key.ToString() == "219")
            {
                FlipSortDirection(sender);
                return;
            }

            var moveDown = e.Key == Windows.System.VirtualKey.Add || e.Key.ToString() == "187";
            var moveUp = e.Key == Windows.System.VirtualKey.Subtract || e.Key.ToString() == "189";
            
            if(!moveUp && !moveDown)
            {
                return;
            }
                        
            HandleMove(sender, moveUp ? MoveDirection.Up : MoveDirection.Down);
        }

        /// <summary>
        /// Handle moving items in one of the two data grids
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="direction">Direction that the entry should be moved</param>
        private void HandleMove(object sender, MoveDirection direction)
        {
            if (sender is GridView { Name: "AvailableGrid" } sgw && SortablePropertyModule?.AvailableSortEntries.Count > 1 && SortablePropertyModule?.SelectedAvailableEntry != null)
            {
                var result = HandleMove(SortablePropertyModule.AvailableSortEntries, SortablePropertyModule.SelectedAvailableEntry, direction);
                if (result >= 0)
                {
                    sgw.SelectedIndex = result;
                }
            }

            if (sender is GridView { Name: "SortGrid" } agw && SortablePropertyModule?.EntriesSortedBy.Count > 1 && SortablePropertyModule?.SelectedSortByEntry != null)
            {
                var result = HandleMove(SortablePropertyModule.EntriesSortedBy, SortablePropertyModule.SelectedSortByEntry, direction);
                if (result >= 0)
                {
                    agw.SelectedIndex = result;
                }
            }
        }

        /// <summary>
        /// Handle moving an item in the collection up and down.
        /// </summary>
        /// <param name="collection">Collection in which the resides</param>
        /// <param name="entryToMove">The entry that should be moved</param>
        /// <param name="direction">The direction in which the entry should be moved</param>
        /// <returns>New index of the entry, unless no entry was moved in which case -1 is returned</returns>
        private static int HandleMove(ObservableCollection<SortOrderEntry> collection, SortOrderEntry entryToMove, MoveDirection direction)
        {
            var index = collection.IndexOf(entryToMove);
            var newIndex = index + (direction == MoveDirection.Up ? -1 : 1);
            if (newIndex >= 0 && newIndex < collection.Count)
            {
                collection.Move(index, newIndex);
                return newIndex;
            }

            return -1;
        }

        /// <summary>
        /// Handle the movement of the selected entry between the available entries and the sorted by entries
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        private void HandleSelectedItemMove(object sender)
        {
            if (sender is GridView { Name: "AvailableGrid" } && SortablePropertyModule?.MoveAvailableEnabled == true)
            {
                SortablePropertyModule.MoveEntryFromAvailableToSortedBy();
            }
            else if (sender is GridView { Name: "SortGrid" } && SortablePropertyModule?.MoveSortedByEnabled == true)
            {
                SortablePropertyModule.MoveEntryFromSortedByToAvailable();
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// Backing variable for the <see cref="SortablePropertyModule"/> property
        /// </summary>
        private SortablePropertyModule? _sortablePropertyModule;

        #endregion
    }
}
