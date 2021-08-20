using Microsoft.UI.Xaml.Controls;
using System;
using System.Text.Json.Serialization;

namespace VaraniumSharp.WinUI.TabViewHelpers
{
    /// <summary>
    /// Readmodel used to store TabView item details
    /// </summary>
    public sealed class TabViewModel
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public TabViewModel()
        {
            Name = Guid.NewGuid().ToString();
            Header = "New Tab";
        }

        /// <summary>
        /// Create a new instance from a <see cref="TabViewItem"/>
        /// </summary>
        /// <param name="item">Tab item to create the persistence model for</param>
        public TabViewModel(TabViewItem item)
        {
            Name = item.Name;
            Header = item.Header?.ToString() ?? string.Empty;
            IsClosable = item.IsClosable;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of the Tab
        /// </summary>
        [JsonInclude]
        public string Header { get; set; }

        /// <summary>
        /// Indicate if the tab item can be closed
        /// </summary>
        [JsonInclude]
        public bool IsClosable { get; set; }

        /// <summary>
        /// The name of the tab item
        /// </summary>
        [JsonInclude]
        public string Name { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new <see cref="TabViewItem"/> based on the model
        /// </summary>
        /// <returns>Populated tab item</returns>
        public TabViewItem GetTabViewItem()
        {
            return new()
            {
                Name = Name,
                Header = Header,
                IsClosable = IsClosable
            };
        }

        #endregion
    }
}