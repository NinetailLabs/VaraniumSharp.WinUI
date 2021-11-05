using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.WinUI.Interfaces.SettingPane;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TestHelper.Settings
{
    public sealed partial class SettingControl2 : ISettingControl
    {
        #region Constructor

        public SettingControl2()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Properties

        public int ControlIndex => 0;

        public string ControlTitle => "My Settings";

        public int NavigationEntryIndex => 1;

        public string NavigationEntryTitle => "User Settings";

        public Symbol NavigationGlyph => Symbol.Permissions;

        #endregion
    }
}
