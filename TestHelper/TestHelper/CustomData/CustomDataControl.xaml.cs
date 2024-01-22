using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.CustomPaneBase;
using VaraniumSharp.WinUI.CustomShaping;
using VaraniumSharp.WinUI.Interfaces.CustomShaping;
using VaraniumSharp.WinUI.Shared.ShapingModule;
using VaraniumSharp.WinUI.ExtensionMethods;

namespace TestHelper.CustomData
{
    [AutomaticContainerRegistration(typeof(CustomDataControl))]
    [DisplayComponent("Custom Control", "37c8c6c6-7072-4540-9d73-09ec3723fd3f", "Sorting", 100, 100, typeof(CustomDataControl))]
    public sealed partial class CustomDataControl : ICustomShapingDisplayComponent
    {
        #region Constructor

        public CustomDataControl()
        {
            InitializeComponent();
            EntriesShapedBy = new();
            TestBlock1.ShowRichText(TextValue1);

            var tokenHelper = new TokenHelper
            {
                TokenRegex = @"(\<[^<>]*\>)",
                BoldStartToken = "<b>",
                BoldEndToken = "</b>",
                ItalicStartToken = "<i>",
                ItalicEndToken = "</i>",
                NewLineToken = "<br>"
            };

            TestBlock2.ShowRichText(TextValue2, tokenHelper);
        }

        #endregion

        #region Events

#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        public event EventHandler? ShapingChanged;

        #endregion

        #region Properties

        public Guid ContentId => Guid.Parse("37c8c6c6-7072-4540-9d73-09ec3723fd3f");

        public bool EnableControl
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                var customJson = new CustomDataControlJson
                {
                    EnableControl = value
                };
                if (EntriesShapedBy.FirstOrDefault(x => x.PropertyName == nameof(EnableControl)) is CustomShapingEntry myEntry)
                {
                    myEntry.CustomData.CustomDataJson = JsonSerializer.Serialize(customJson, CustomDataControlJsonContext.Default.CustomDataControlJson);
                }
                else
                {
                    var entry = new CustomShapingEntry("CustomEntry")
                    {
                        PropertyName = nameof(EnableControl),
                        CustomData = new CustomShapingData
                        {
                            CustomDataJson = JsonSerializer.Serialize(customJson)
                        }
                    };
                    EntriesShapedBy.Add(entry);
                }

                
                ShapingChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public List<ShapingEntry> EntriesShapedBy { get; }
        public Guid InstanceId { get; set; }
        public bool ShowResizeHandle { get; set; }
        public bool StartupLoad { get; set; }

        public string TextValue1 => "This is some {b}bold{/b} text.{br}{br}This is a new paragraph.";

        public string TextValue2 => "This is some <b>bold</b> text.<br><br>This is a new paragraph.";

        public string Title { get; set; } = "Custom Control";

        #endregion

        #region Public Methods

        public Task InitAsync()
        {
            // Not used for now
            return Task.CompletedTask;
        }

        public void InitFilterOrder(List<CustomEntryStorageModel> filterEntries)
        {
            foreach (var entry in filterEntries)
            {
                if (entry is {PropertyName: nameof(EnableControl), CustomData: { }})
                {
                    var data = JsonSerializer.Deserialize(entry.CustomData.CustomDataJson, CustomDataControlJsonContext.Default.CustomDataControlJson);
                    EnableControl = data?.EnableControl ?? false;
                }
            }
        }

        public Task SetControlSizeAsync(double width, double height)
        {
            Width = width;
            Height = height;
            return Task.CompletedTask;
        }

        #endregion

        #region Variables

        private bool _isEnabled;

        #endregion
    }
}
