using Microsoft.UI.Xaml.Controls;

namespace VaraniumSharp.WinUI.SettingPane
{
    /// <summary>
    /// Readmodel used to handle the setting categories
    /// </summary>
    public class SettingCategory
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SettingCategory()
        {
            Name = string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Glyph to display for the category
        /// </summary>
        public Symbol Glyph { get; init; }

        /// <summary>
        /// Desired Index for the category
        /// </summary>
        public int Index { get; init; }

        /// <summary>
        /// Name of the category
        /// </summary>
        public string Name { get; init; }

        #endregion
    }
}