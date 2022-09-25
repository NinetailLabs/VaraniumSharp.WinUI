using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;
using VaraniumSharp.WinUI.Interfaces.Pickers;

namespace TestHelper.FilePicking
{
    [AutomaticContainerRegistration(typeof(FilePicking))]
    [DisplayComponent("File Picker", "2091c702-ee8c-4b04-baff-49418906a783", "File Handling", 100, 100, typeof(FilePicking))]
    public sealed partial class FilePicking : IDisplayComponent
    {
        #region Constructor

        public FilePicking(IPickerWrapper pickerWrapper)
        {
            _pickerWrapper = pickerWrapper;
            InitializeComponent();
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Properties

        public Guid ContentId => Guid.Parse("2091c702-ee8c-4b04-baff-49418906a783");
        public Guid InstanceId { get; set; }
        public bool ShowResizeHandle { get; set; }
        public bool StartupLoad { get; set; }
        public string Title { get; set; } = "File Picker";

        #endregion

        #region Public Methods

        public Task InitAsync()
        {
            return Task.CompletedTask;
        }

        public Task SetControlSizeAsync(double width, double height)
        {
            Width = width;
            Height = height;
            return Task.CompletedTask;
        }

        #endregion

        #region Private Methods

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var file = await _pickerWrapper.PickSingleFileToOpenAsync(new List<string> {".txt"});
        }

        #endregion

        #region Variables

        private readonly IPickerWrapper _pickerWrapper;

        #endregion
    }
}
