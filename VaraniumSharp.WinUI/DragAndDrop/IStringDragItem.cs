namespace VaraniumSharp.WinUI.DragAndDrop
{
    /// <summary>
    /// Interface used for string Id based drag and drop operations
    /// </summary>
    public interface IStringDragItem
    {
        /// <summary>
        /// The string used to identify the drag item
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// The type of entry
        /// </summary>
        string EntryType { get; }
    }
}
