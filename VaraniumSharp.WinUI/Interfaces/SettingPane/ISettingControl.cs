using Microsoft.UI.Xaml.Controls;
using VaraniumSharp.Attributes;

namespace VaraniumSharp.WinUI.Interfaces.SettingPane
{
    /// <summary>
    /// Interface used to denote a control as being a setting control
    /// </summary>
    [AutomaticConcretionContainerRegistration]
    public interface ISettingControl
    {
        #region Properties

        /// <summary>
        /// Desired index of the setting on the page.
        /// If there are multiple entries requesting the same index the entries will be sorted alphabetically
        /// </summary>
        public int ControlIndex { get; }

        /// <summary>
        /// The title of the setting control
        /// </summary>
        public string ControlTitle { get; }

        /// <summary>
        /// Desired index of the navigation entry.
        /// Note that if there are multiple entries the first entry will take precedence when the order is set
        /// </summary>
        public int NavigationEntryIndex { get; }

        /// <summary>
        /// The title of the navigation entry the control belongs to
        /// </summary>
        public string NavigationEntryTitle { get; }

        /// <summary>
        /// The glyph to show for the navigation entry
        /// </summary>
        public Symbol NavigationGlyph { get; }

        #endregion
    }
}