using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.WinUI.DragAndDrop;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.GroupModule
{
    /// <summary>
    /// Control that provides drag and drop grouping
    /// </summary>
    public sealed partial class GroupControl : INotifyPropertyChanged
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public GroupControl()
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
        /// DragModule used for drag and drop from the grouped by entries to the available entries
        /// </summary>
        public DragModule<ShapingEntry>? AvailableDragModule { get; private set; }

        /// <summary>
        /// DragModule used for drag and drop from the available entries to the grouped by entries
        /// </summary>
        public DragModule<ShapingEntry>? GroupDragModule { get; private set; }

        /// <summary>
        /// Module that contains the grouping properties
        /// </summary>
        public GroupingPropertyModule? GroupingPropertyModule
        {
            get => _groupingPropertyModule;
            set
            {
                _groupingPropertyModule = value;
                if (value != null)
                {
                    GroupDragModule = new("GroupEntry", DataPackageOperation.Move, value.AvailableShapingEntries, value.EntriesShapedBy);
                    AvailableDragModule = new("GroupEntry", DataPackageOperation.Move, value.EntriesShapedBy, value.AvailableShapingEntries);
                }
                else
                {
                    GroupDragModule = null;
                    AvailableDragModule = null;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Occurs when a key is pressed while the user is in either of the sort grids
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridView_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                HandleSelectedItemMove(sender);
                return;
            }

            var moveDown = e.Key == Windows.System.VirtualKey.Add || e.Key.ToString() == "187";
            var moveUp = e.Key == Windows.System.VirtualKey.Subtract || e.Key.ToString() == "189";

            if (!moveUp && !moveDown)
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
            if (sender is GridView { Name: "AvailableGrid" } sgw && GroupingPropertyModule?.AvailableShapingEntries.Count > 1 && GroupingPropertyModule?.SelectedAvailableEntry != null)
            {
                var result = HandleMove(GroupingPropertyModule.AvailableShapingEntries, GroupingPropertyModule.SelectedAvailableEntry, direction);
                if (result >= 0)
                {
                    sgw.SelectedIndex = result;
                }
            }

            if (sender is GridView { Name: "SortGrid" } agw && GroupingPropertyModule?.EntriesShapedBy.Count > 1 && GroupingPropertyModule?.SelectedShapedByEntry != null)
            {
                var result = HandleMove(GroupingPropertyModule.EntriesShapedBy, GroupingPropertyModule.SelectedShapedByEntry, direction);
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
        private static int HandleMove(ObservableCollection<ShapingEntry> collection, ShapingEntry entryToMove, MoveDirection direction)
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
            if (sender is GridView { Name: "AvailableGrid" } && GroupingPropertyModule?.MoveAvailableEnabled == true)
            {
                GroupingPropertyModule.MoveEntryFromAvailableToShapedBy();
            }
            else if (sender is GridView { Name: "SortGrid" } && GroupingPropertyModule?.MoveShapedByEnabled == true)
            {
                GroupingPropertyModule.MoveEntryFromShapedByToAvailable();
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// Backing variable for the <see cref="GroupingPropertyModule"/> property
        /// </summary>
        private GroupingPropertyModule? _groupingPropertyModule;

        #endregion
    }
}
