using System.Collections.Generic;
using System.Linq;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.FilterModule
{
    /// <summary>
    /// Filter entry to store
    /// </summary>
    public sealed class FilterEntryStorageModel : ShapingEntryStorageModelBase
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FilterEntryStorageModel()
        {
            CurrentFilters = new();
        }

        /// <summary>
        /// Construct and populate
        /// </summary>
        /// <param name="shapingEntry">Entry used for filtering</param>
        public FilterEntryStorageModel(FilterShapingEntry shapingEntry)
            : base(shapingEntry)
        {
            CurrentFilters = shapingEntry.CurrentFilterValues.ToList();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Contains the list of current filter values
        /// </summary>
        public List<string> CurrentFilters { get; set; }

        #endregion
    }
}