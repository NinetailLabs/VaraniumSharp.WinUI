/*
 * Original code is from an issue on the WindowsCommunityToolkit GitHub.
 * The issue is here: https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3089
 * Code link: https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3089#issuecomment-604256939
 *
 * Code has been cleaned up and modified as required.
 * 
 */

using Windows.Foundation.Collections;
using Microsoft.UI.Xaml.Data;

namespace VaraniumSharp.WinUI.Collections
{
    /// <summary>
    /// Contains the entries for a grouped collection
    /// </summary>
    public class CollectionViewGroup : ICollectionViewGroup
    {
        #region Constructor

        /// <summary>
        /// Construct and populate with group Id
        /// </summary>
        /// <param name="group">The Id of the group</param>
        public CollectionViewGroup(object group)
        {
            Group = group;
            Items = new ObservableVector<object>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The Id of the group
        /// </summary>
        public object Group { get; }

        /// <summary>
        /// Collection containing the group items
        /// </summary>
        public IObservableVector<object> GroupItems => Items;

        /// <summary>
        /// Typed collection containing the group items
        /// </summary>
        public ObservableVector<object> Items { get; }

        /// <summary>
        /// The starting index of entries in the collection when the group is used as part of a larger collection
        /// </summary>
        public int StartIndex { get; set; }

        #endregion
    }
}