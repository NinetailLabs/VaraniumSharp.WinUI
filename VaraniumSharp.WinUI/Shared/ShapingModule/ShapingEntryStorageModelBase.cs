namespace VaraniumSharp.WinUI.Shared.ShapingModule
{
    /// <summary>
    /// Base class used to store shaping entries for persistence
    /// </summary>
    public abstract class ShapingEntryStorageModelBase
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        protected ShapingEntryStorageModelBase()
        {
            PropertyName = string.Empty;
        }

        /// <summary>
        /// Construct and populate
        /// </summary>
        /// <param name="shapingEntry">Shaping entry to use for populating</param>
        protected ShapingEntryStorageModelBase(ShapingEntry shapingEntry)
        {
            PropertyName = shapingEntry.PropertyName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the property to sort by
        /// </summary>
        public string PropertyName { get; set; }

        #endregion
    }
}