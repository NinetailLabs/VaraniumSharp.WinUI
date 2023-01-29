using System.Collections.ObjectModel;
using System.Collections.Specialized;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.FilterModule
{
    /// <inheritdoc />
    public class FilterShapingEntry : ShapingEntry
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="entryType"></param>
        public FilterShapingEntry(string entryType) 
            : base(entryType)
        {
            CurrentFilterValues = new();
            CurrentFilterValues.CollectionChanged += CurrentFilterValuesOnCollectionChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Collection of strings representing the applied filters
        /// </summary>
        public ObservableCollection<string> CurrentFilterValues { get; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Fired when the <see cref="CurrentFilterValues"/> collection changes
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void CurrentFilterValuesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RequestShapingUpdateEvent();
        }

        #endregion
    }
}