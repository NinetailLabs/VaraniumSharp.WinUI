using CommunityToolkit.WinUI.UI;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Data about properties used to sort collection
    /// </summary>
    public class SortableShapingEntry : ShapingEntry
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SortableShapingEntry()
            : base("SortEntry")
        {
            SetDirectionIcon();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The default direction in which the entry is ordered
        /// </summary>
        public SortDirection DefaultDirection { get; init; }

        /// <summary>
        /// Icon to indicate direction
        /// </summary>
        public string DirectionIcon { get; private set; } = null!;

        /// <summary>
        /// The current direction the list should be sorted in
        /// </summary>
        public SortDirection SortDirection
        {
            get => _sortDirection;
            set
            {
                _sortDirection = value;
                SetDirectionIcon();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Used to change the direction the control shapes in
        /// </summary>
        public void ChangeDirectionClick()
        {
            SortDirection = SortDirection == SortDirection.Ascending
                ? SortDirection.Descending
                : SortDirection.Ascending;
            RequestShapingUpdateEvent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the sort icon to the correct one
        /// </summary>
        private void SetDirectionIcon()
        {
            DirectionIcon = SortDirection == SortDirection.Ascending
                ? "Asc"
                : "Desc";
        }

        #endregion

        #region Variables

        /// <summary>
        /// Backing variable for the <see cref="SortDirection"/> property
        /// </summary>
        private SortDirection _sortDirection;

        #endregion
    }
}