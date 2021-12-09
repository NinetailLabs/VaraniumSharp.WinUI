using System.ComponentModel;
using Microsoft.UI.Xaml;
using VaraniumSharp.WinUI.Shared.ShapingControlHelper;

namespace VaraniumSharp.WinUI.GroupModule
{
    /// <summary>
    /// Control that provides drag and drop grouping
    /// </summary>
    public sealed partial class GroupControl : INotifyPropertyChanged
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public GroupControl()
        {
            InitializeComponent();
            ControlHelper = new("GroupEntry");
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
        public ShapingControlHelper<GroupingPropertyModule, GroupingPropertyAttribute> ControlHelper { get; }

        /// <summary>
        /// Module that contains the grouping properties
        /// </summary>
        public GroupingPropertyModule? GroupingPropertyModule
        {
            get => ControlHelper.ShapingPropertyModule;
            set=> ControlHelper.ShapingPropertyModule = value;
        }

        /// <summary>
        /// The width of the container inside the scroll container
        /// </summary>
        private double InnerContainerWidth { get; set; }

        /// <summary>
        /// The width of the container
        /// </summary>
        private double ScrollContainerWidth { get; set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Occurs when a key is pressed while the user is in either of the sort grids
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridView_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            ControlHelper.SharedKeyUpAction(sender, e);
        }

        /// <summary>
        /// Occurs when the size of the control changes
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void GroupControl_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollContainerWidth = PrimaryColumn.ActualWidth - 2;
            InnerContainerWidth = PrimaryColumn.ActualWidth - 10;
        }

        #endregion
    }
}
