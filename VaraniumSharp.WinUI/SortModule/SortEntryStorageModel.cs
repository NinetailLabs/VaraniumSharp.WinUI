using CommunityToolkit.WinUI.UI;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Sort entry to store
    /// </summary>
    public sealed class SortEntryStorageModel : ShapingEntryStorageModelBase
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SortEntryStorageModel()
        {
        }

        /// <summary>
        /// Construct and populate
        /// </summary>
        /// <param name="sortEntry">Sort entry to populate from</param>
        public SortEntryStorageModel(SortableShapingEntry sortEntry)
            : base(sortEntry)
        {
            SortDirection = sortEntry.SortDirection;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The direction to sort by
        /// </summary>
        public SortDirection SortDirection { get; set; }

        #endregion
    }
}
