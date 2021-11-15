namespace VaraniumSharp.WinUI.DragAndDrop
{
    /// <summary>
    /// Helper used for dragging and dropping
    /// </summary>
    public class DragData
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DragData()
        {
            DataType = string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type of data being dragged
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Unique int identifier for the entry being dragged.
        /// This entry can be null if the user instead wants to use the <see cref="StringIdentifier"/>
        /// </summary>
        public int? IntegerIdentifier { get; set; }

        /// <summary>
        /// Unique string identifier for the entry being dragged.
        /// This entry can be null if the user instead wants to use the <see cref="IntegerIdentifier"/>
        /// </summary>
        public string? StringIdentifier { get; set; }

        #endregion
    }
}
