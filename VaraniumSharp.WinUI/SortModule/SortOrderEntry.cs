using System;
using System.ComponentModel;
using CommunityToolkit.WinUI.UI;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Data about properties used for sorting
    /// </summary>
    public class SortOrderEntry : INotifyPropertyChanged
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SortOrderEntry()
        {
            PropertyName = string.Empty;
            SortHeader = string.Empty;
            SortTooltip = string.Empty;
            SetSortIcon();
        }

        #endregion

        #region Events

        /// <inheritdoc/>
#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        /// <summary>
        /// Request that sort order is updated
        /// </summary>
        public event EventHandler? RequestSortUpdate;

        #endregion

        #region Properties

        /// <summary>
        /// The default direction in which the entry is sorted
        /// </summary>
        public SortDirection DefaultSortDirection { get; init; }

        /// <summary>
        /// Name of the property that the information is for
        /// </summary>
        public string PropertyName { get; init; }

        /// <summary>
        /// The current direction the list should be sorted in
        /// </summary>
        public SortDirection SortDirection
        {
            get => _sortDirection;
            set
            {
                _sortDirection = value;
                SetSortIcon();
            }
        }

        /// <summary>
        /// Header for the sort control
        /// </summary>
        public string SortHeader { get; init; }

        /// <summary>
        /// Icon to indicate sort direction
        /// </summary>
        public string SortIcon { get; private set; } = null!; // TODO - Replace with icon

        /// <summary>
        /// Tooltip for the sort order
        /// </summary>
        public string SortTooltip { get; init; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Used to change the direction the control sorts in
        /// </summary>
        public void ChangeDirectionClick()
        {
            SortDirection = SortDirection == SortDirection.Ascending
                ? SortDirection.Descending
                : SortDirection.Ascending;
            RequestSortUpdate?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the sort icon to the correct one
        /// </summary>
        private void SetSortIcon()
        {
            // TODO - Update with icons
            SortIcon = SortDirection == SortDirection.Ascending
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