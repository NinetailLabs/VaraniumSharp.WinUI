using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.WinUI.Interfaces.SettingPane;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TestHelper.Settings
{
    public sealed partial class SettingControl3 : ISettingControl
    {
        #region Constructor

        public SettingControl3()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Properties

        public int ControlIndex => 0;

        public string ControlTitle => "Advanced Settings";

        public int NavigationEntryIndex => 0;

        public string NavigationEntryTitle => "Test Settings";

        public Symbol NavigationGlyph => Symbol.Calendar;

        #endregion
    }
}
