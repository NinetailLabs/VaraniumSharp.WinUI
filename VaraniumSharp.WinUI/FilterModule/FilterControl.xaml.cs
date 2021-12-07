using System.ComponentModel;

namespace VaraniumSharp.WinUI.FilterModule
{
    /// <summary>
    /// Control that provides easy access to filtering controls
    /// </summary>
    public sealed partial class FilterControl : INotifyPropertyChanged
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FilterControl()
        {
            InitializeComponent();
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
        /// Module used to handle the filtering
        /// </summary>
        public FilterablePropertyModule? FilterablePropertyModule { get; set; }

        #endregion
    }
}
