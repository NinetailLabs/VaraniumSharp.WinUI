using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using VaraniumSharp.WinUI.Shared.ShapingControlHelper;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Control that provides drag and drop sorting
    /// </summary>
    public sealed partial class SortControl : INotifyPropertyChanged
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SortControl()
        {
            InitializeComponent();
            ControlHelper = new("SortEntry");
        }

        #endregion

        #region Events

        /// <inheritdoc />
#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        #endregion

        #region Properties

        /// <summary>
        /// Control helper instance
        /// </summary>
        public ShapingControlHelper<SortablePropertyModule, SortablePropertyAttribute> ControlHelper { get; }

        /// <summary>
        /// Pass-through for the sortable module
        /// </summary>
        public SortablePropertyModule? SortablePropertyModule
        {
            get => ControlHelper.ShapingPropertyModule;
            set => ControlHelper.ShapingPropertyModule = value;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Change the direction in which the selected item is sorted
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        private void FlipSortDirection(object sender)
        {
            if (sender is GridView { Name: "AvailableGrid" })
            {
                (ControlHelper.ShapingPropertyModule?.SelectedAvailableEntry as SortableShapingEntry)?.ChangeDirectionClick();
            }
            
            if (sender is GridView { Name: "SortGrid" })
            {
                (ControlHelper.ShapingPropertyModule?.SelectedShapedByEntry as SortableShapingEntry)?.ChangeDirectionClick();
            }
        }

        /// <summary>
        /// Occurs when a key is pressed while the user is in either of the sort grids
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridView_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            ControlHelper.SharedKeyUpAction(sender, e);

            if(e.Key.ToString() == "221" || e.Key.ToString() == "219")
            {
                FlipSortDirection(sender);
            }
        }

        #endregion
    }
}
