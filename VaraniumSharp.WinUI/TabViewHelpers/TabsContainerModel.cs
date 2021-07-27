using System.Collections.Generic;

namespace VaraniumSharp.WinUI.TabViewHelpers
{
    /// <summary>
    /// Container used to persist the <see cref="TabViewModel"/> entries
    /// </summary>
    public sealed class TabsContainerModel
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public TabsContainerModel()
        {
            Tabs = new List<TabViewModel>();
        }

        /// <summary>
        /// Constructor and set the <see cref="Tabs"/>
        /// </summary>
        /// <param name="tabs">Tabs to add to the collection</param>
        public TabsContainerModel(IEnumerable<TabViewModel> tabs)
        {
            Tabs = new List<TabViewModel>(tabs);
        }

        /// <summary>
        /// Collection of Tabs
        /// </summary>
        public List<TabViewModel> Tabs { get; set; }
    }
}
