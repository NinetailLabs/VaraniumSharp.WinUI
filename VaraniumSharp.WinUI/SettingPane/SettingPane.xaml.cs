using System;
using System.ComponentModel;
using System.Threading.Tasks;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.SettingPane;

namespace VaraniumSharp.WinUI.SettingPane
{
    /// <summary>
    /// Pane that stores settings
    /// </summary>
    [DisplayComponent("Settings", ContentIdentifier, "Settings", 100, 100, typeof(ISettingPane), ShowInMenus = false)]
    [AutomaticContainerRegistration(typeof(ISettingPane))]
    public sealed partial class SettingPane : ISettingPane
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public SettingPane()
        {
            InitializeComponent();
            Title = "Settings";
        }

        #endregion

        #region Events

        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Properties

        /// <inheritdoc />
        public Guid ContentId => Guid.Parse(ContentIdentifier);

        /// <inheritdoc />
        public bool ShowResizeHandle { get; set; }

        /// <inheritdoc />
        public bool StartupLoad { get; set; }

        /// <inheritdoc />
        public string Title { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public Task InitAsync()
        {
            // Does nothing for now
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task SetControlSizeAsync(double width, double height)
        {
            Width = width;
            Height = height;

            return Task.CompletedTask;
        }

        #endregion

        #region Variables

        /// <summary>
        /// Control identifier
        /// </summary>
        private const string ContentIdentifier = "90ef7c67-1cea-4001-aedc-afb8c760a4c8";

        #endregion
    }
}
