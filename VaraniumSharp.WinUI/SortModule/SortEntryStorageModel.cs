using CommunityToolkit.WinUI.UI;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Sort entry to store
    /// </summary>
    public class SortEntryStorageModel
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SortEntryStorageModel()
        {
            PropertyName = string.Empty;
        }

        /// <summary>
        /// Construct and populate
        /// </summary>
        /// <param name="sortEntry">Sort entry to populate from</param>
        public SortEntryStorageModel(SortableShapingEntry sortEntry)
        {
            PropertyName = sortEntry.PropertyName;
            SortDirection = sortEntry.SortDirection;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the property to sort by
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// The direction to sort by
        /// </summary>
        public SortDirection SortDirection { get; set; }

        #endregion
    }
}
