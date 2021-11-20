using System.Collections.ObjectModel;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.WinUI.DragAndDrop;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.Shared.ShapingControlHelper
{
    /// <summary>
    /// Assist with control that have an available and shaping collection
    /// </summary>
    public class ShapingControlHelper<T, TD> where T: ShapingPropertyModuleBase<TD> where TD : ShapingPropertyAttributeBase
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dragType">The string identifying the drag entry type</param>
        public ShapingControlHelper(string dragType)
        {
            _dragType = dragType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// DragModule used for drag and drop from the sorted by entries to the available entries
        /// </summary>
        public DragModule<ShapingEntry>? AvailableDragModule { get; private set; }

        /// <summary>
        /// DragModule used for drag and drop from the available entries to the sorted by entries
        /// </summary>
        public DragModule<ShapingEntry>? ShapedDragModule { get; private set; }

        /// <summary>
        /// Module that contains the shaping properties
        /// </summary>
        public T? ShapingPropertyModule
        {
            get => _shapingPropertyModule;
            set
            {
                _shapingPropertyModule = value;
                if (value != null)
                {
                    ShapedDragModule = new(_dragType, DataPackageOperation.Move, value.AvailableShapingEntries, value.EntriesShapedBy);
                    AvailableDragModule = new(_dragType, DataPackageOperation.Move, value.EntriesShapedBy, value.AvailableShapingEntries);
                }
                else
                {
                    ShapedDragModule = null;
                    AvailableDragModule = null;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Assist with handling basic KeyUp action for the control
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        public void SharedKeyUpAction(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
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

        #endregion

        #region Private Methods

        /// <summary>
        /// Handle moving items in one of the two data grids
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="direction">Direction that the entry should be moved</param>
        private void HandleMove(object sender, MoveDirection direction)
        {
            if (sender is GridView { Name: "AvailableGrid" } sgw && ShapingPropertyModule?.AvailableShapingEntries.Count > 1 && ShapingPropertyModule?.SelectedAvailableEntry != null)
            {
                var result = HandleMove(ShapingPropertyModule.AvailableShapingEntries, ShapingPropertyModule.SelectedAvailableEntry, direction);
                if (result >= 0)
                {
                    sgw.SelectedIndex = result;
                }
            }

            if (sender is GridView { Name: "SortGrid" } agw && ShapingPropertyModule?.EntriesShapedBy.Count > 1 && ShapingPropertyModule?.SelectedShapedByEntry != null)
            {
                var result = HandleMove(ShapingPropertyModule.EntriesShapedBy, ShapingPropertyModule.SelectedShapedByEntry, direction);
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
            if (sender is GridView { Name: "AvailableGrid" } && ShapingPropertyModule?.MoveAvailableEnabled == true)
            {
                ShapingPropertyModule.MoveEntryFromAvailableToShapedBy();
            }
            else if (sender is GridView { Name: "SortGrid" } && ShapingPropertyModule?.MoveShapedByEnabled == true)
            {
                ShapingPropertyModule.MoveEntryFromShapedByToAvailable();
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// The string identifying the drag entry type
        /// </summary>
        private readonly string _dragType;

        /// <summary>
        /// Backing variable for the <see cref="ShapingPropertyModule"/> property
        /// </summary>
        private T? _shapingPropertyModule;

        #endregion
    }
}