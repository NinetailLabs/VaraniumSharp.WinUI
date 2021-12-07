using System;
using System.Collections.Generic;

namespace VaraniumSharp.WinUI.FilterModule
{
    /// <summary>
    /// Interface for filter controls
    /// </summary>
    internal interface IFilterControl
    {
        #region Events

        /// <summary>
        /// Event used to request filter refresh
        /// </summary>
        event EventHandler RefreshFiltering;

        #endregion

        #region Properties

        /// <summary>
        /// The name to display for the filter
        /// </summary>
        public string FilterDisplayName { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Function that is used to filter the entries
        /// </summary>
        /// <param name="obj">Object that should be inspected</param>
        /// <returns>True if object matches the filter, otherwise false</returns>
        bool Filter(object obj);

        /// <summary>
        /// Apply a filter on the control.
        ///  </summary>
        /// <param name="entries">List of entries to filter on</param>
        /// <returns>Indicate whether each of the provided filter values was applied or not</returns>
        List<KeyValuePair<object, FilterState>> FilterBy(List<object> entries);

        #endregion
    }
}