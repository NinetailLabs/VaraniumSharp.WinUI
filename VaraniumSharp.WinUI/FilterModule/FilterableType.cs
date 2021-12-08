namespace VaraniumSharp.WinUI.FilterModule
{
    /// <summary>
    /// Types that can be used in filtering
    /// </summary>
    public enum FilterableType
    {
        /// <summary>
        /// A regular string property.
        /// This produces a text box the user can enter a filter value into
        /// </summary>
        SearchableString,

        /// <summary>
        /// A string property that can only be a set of pre-defined values.
        /// This produces a dropdown that the user selects values from.
        /// When using this type ensure that the constructor with className and filterListProperty is used
        /// </summary>
        PredefinedString,

        /// <summary>
        /// An enumeration property.
        /// This produces a dropdown that the user selects values from.
        /// </summary>
        Enumeration,

        /// <summary>
        /// A boolean property.
        /// This produces a dropdown that the user selects values from.
        /// </summary>
        Boolean
    }
}