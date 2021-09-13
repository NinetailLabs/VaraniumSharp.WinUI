using System.Collections.ObjectModel;
using System.ComponentModel;
using VaraniumSharp.WinUI.SettingPane;

namespace VaraniumSharp.WinUI.Interfaces.SettingPane
{
    /// <summary>
    /// Context for the setting pane
    /// </summary>
    public interface ISettingPaneContext : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// The currently selected index
        /// </summary>
        SettingCategory? SelectedCategory { get; set; }

        /// <summary>
        /// Collection of setting controls grouped by their Navigation items
        /// </summary>
        ObservableCollection<SettingCategory> SettingCategories { get; }

        /// <summary>
        /// Collection of controls for the selected setting category
        /// </summary>
        ObservableCollection<ISettingControl> SettingControls { get; }

        #endregion
    }
}