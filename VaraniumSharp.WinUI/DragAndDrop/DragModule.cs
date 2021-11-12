using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using VaraniumSharp.Logging;
using Windows.ApplicationModel.DataTransfer;

namespace VaraniumSharp.WinUI.DragAndDrop
{
    /// <summary>
    /// Assists with drag and drop operations
    /// </summary>
    public class DragModule<T> where T: IStringDragItem
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="entryTypeToHandler">The type of entry operation that should be handled</param>
        /// <param name="acceptedOperation">The type of package operations that can be handled</param>
        /// <param name="sourceCollection">Collection where entries for the drag operation are coming from</param>
        /// <param name="targetCollection">Collection where entries from the drag should be added to</param>
        public DragModule(string entryTypeToHandler, DataPackageOperation acceptedOperation, ObservableCollection<T> sourceCollection, ObservableCollection<T> targetCollection)
        {
            _logger = StaticLogger.GetLogger<DragModule<T>>();
            _entryTypeToHandler = entryTypeToHandler;
            _acceptedOperation = acceptedOperation;
            _sourceCollection = sourceCollection;
            _targetCollection = targetCollection;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start the dragging of an entry that implements the <see cref="IStringDragItem"/> type
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">DragItemStarting arguments</param>
        public void OnStringTypeDragItemStarting(object sender, DragItemsStartingEventArgs e)
        {
            var dragEntries = new List<DragData>();
            foreach (var item in e.Items)
            {
                if (item is IStringDragItem sortItem)
                {
                    dragEntries.Add(new DragData
                    {
                        DataType = sortItem.EntryType,
                        StringIdentifier = sortItem.Identifier
                    });
                }
            }

            e.Data.SetText(JsonSerializer.Serialize(new DragDataCollection(dragEntries)));
            e.Data.RequestedOperation = DataPackageOperation.Move;
        }

        /// <summary>
        /// Occurs when the user drags an entry over the control.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">DragEvent arguments</param>
        public async void OnStringTypeDragOver(object sender, DragEventArgs e)
        {
            if (!e.DataView.Contains(StandardDataFormats.Text))
            {
                e.AcceptedOperation = DataPackageOperation.None;
                return;
            }

            var def = e.GetDeferral();
            try
            {
                var jsonData = await e.DataView.GetTextAsync();
                var dragData = JsonSerializer.Deserialize(jsonData, DragDataCollectionJsonContext.Default.DragDataCollection);
                e.AcceptedOperation = dragData?.EntryType == _entryTypeToHandler 
                    ? _acceptedOperation 
                    : DataPackageOperation.None;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred during the DragOver operation");
                e.AcceptedOperation = DataPackageOperation.None;
            }

            def.Complete();
        }

        /// <summary>
        /// Used to handle drop operations on a DataGrid where the internal panel is set to horizontal layout.
        /// Note that this method will produce weird results when the view is horizontal
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Drag arguments</param>
        public async void OnStringTypeVerticalDataGridDrop(object sender, DragEventArgs e)
        {
            if (!e.DataView.Contains(StandardDataFormats.Text))
            {
                e.AcceptedOperation = DataPackageOperation.None;
                return;
            }

            var def = e.GetDeferral();
            try
            {
                var jsonData = await e.DataView.GetTextAsync();
                var dragData = JsonSerializer.Deserialize(jsonData, DragDataCollectionJsonContext.Default.DragDataCollection);

                var target = (GridView)sender;
                var pos = e.GetPosition(target.ItemsPanelRoot);
                var sampleItem = target.ContainerFromIndex(0) as GridViewItem;
                var index = 0;
                if (sampleItem != null)
                {
                    var itemHeight = sampleItem.ActualHeight + sampleItem.Margin.Top + sampleItem.Margin.Bottom;
                    index = Math.Min(target.Items.Count - 1, (int)(pos.Y / itemHeight));

                    var targetItem = (GridViewItem)target.ContainerFromIndex(index);

                    var positionInItem = e.GetPosition(targetItem);
                    if (positionInItem.Y > itemHeight / 2)
                    {
                        index++;
                    }

                    index = Math.Min(target.Items.Count, index);
                }

                foreach (var item in dragData?.Collection ?? new())
                {
                    var entryToMove = _sourceCollection.FirstOrDefault(x => x.Identifier == item.StringIdentifier);
                    if (entryToMove != null)
                    {
                        
                        _sourceCollection.Remove(entryToMove);
                        _targetCollection.Insert(index, entryToMove);
                        index++;
                    }
                }
                e.AcceptedOperation = DataPackageOperation.Move;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred during the drop operation");
            }

            def.Complete();
        }

        #endregion

        #region Variables

        /// <summary>
        /// The type of operation that is accepted
        /// </summary>
        private readonly DataPackageOperation _acceptedOperation;

        /// <summary>
        /// The type of entries that the DragModule handles
        /// </summary>
        private readonly string _entryTypeToHandler;

        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The collection that items should be moved from
        /// </summary>
        private readonly ObservableCollection<T> _sourceCollection;

        /// <summary>
        /// The collection items should be moved to
        /// </summary>
        private readonly ObservableCollection<T> _targetCollection;

        #endregion
    }
}
