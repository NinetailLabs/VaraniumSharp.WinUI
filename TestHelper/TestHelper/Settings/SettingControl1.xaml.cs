﻿using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.WinUI.Interfaces.SettingPane;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TestHelper.Settings
{
    public sealed partial class SettingControl1 : ISettingControl
    {
        #region Constructor

        public SettingControl1()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Properties

        public int ControlIndex => 0;

        public string ControlTitle => "Basic Settings";

        public int NavigationEntryIndex => 0;

        public string NavigationEntryTitle => "Test Settings";

        public Symbol NavigationGlyph => Symbol.Calendar;

        #endregion
    }
}
