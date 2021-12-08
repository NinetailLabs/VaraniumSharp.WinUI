namespace VaraniumSharp.WinUI.FilterModule
{
    /// <summary>
    /// Enum used to indicate the state of the filter when it is requested to be applied programatically
    /// </summary>
    public enum FilterState
    {
        /// <summary>
        /// The filter was applied to the collection
        /// </summary>
        Applied,

        /// <summary>
        /// The filter was removed from the collection
        /// </summary>
        Removed,

        /// <summary>
        /// The requested filter value could not be found
        /// </summary>
        NotFound,

        /// <summary>
        /// The type of filter requested was not valid for the filter type
        /// </summary>
        NotValid,

        /// <summary>
        /// The filter was ignored.
        /// This response is given if multiple filters are passed to a control that can only apply a single value.
        /// </summary>
        Ignored
    }
}