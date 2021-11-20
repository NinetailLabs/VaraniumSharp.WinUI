using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.GroupModule
{
    /// <summary>
    /// Grouping entry to store
    /// </summary>
    public class GroupEntryStorageModel : ShapingEntryStorageModelBase
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public GroupEntryStorageModel()
        {

        }

        /// <summary>
        /// Construct and populate
        /// </summary>
        /// <param name="groupShapingEntry">The group entry to store</param>
        public GroupEntryStorageModel(GroupShapingEntry groupShapingEntry)
            : base(groupShapingEntry)
        {}

        #endregion
    }
}