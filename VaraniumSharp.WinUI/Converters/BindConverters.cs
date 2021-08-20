using Microsoft.UI.Xaml;

namespace VaraniumSharp.WinUI.Converters
{
    /// <summary>
    /// A variety of Converters for use with x:Bind to convert types as required
    /// </summary>
    public static class BindConverters
    {
        #region Public Methods

        /// <summary>
        /// Convert a boolean value to a GridLength.
        /// This is useful to convert a boolean value (should a control be shown) to a grid size for showing/hiding the control
        /// </summary>
        /// <param name="showControls">The boolean value to convert</param>
        /// <param name="trueInt">The length of the grid if the valueToConvert is true</param>
        /// <returns>The GridLength of the trueInt value if the showControls is true, otherwise a GridLength of 0</returns>
        public static GridLength ConvertBoolToDisplaySize(bool showControls, int trueInt) => showControls
            ? new GridLength(trueInt)
            : new GridLength(0);

        /// <summary>
        /// Convert a boolean value to a <see cref="Visibility"/> value
        /// </summary>
        /// <param name="boolToConvert">Value to convert</param>
        /// <returns>Visible if true, otherwise Collapsed</returns>
        public static Visibility ConvertBoolToVisibility(bool boolToConvert) => boolToConvert
            ? Visibility.Visible
            : Visibility.Collapsed;

        #endregion
    }
}
