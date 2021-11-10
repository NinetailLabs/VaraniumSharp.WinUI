using System.Collections.Generic;
using System.Linq;

namespace VaraniumSharp.WinUI.DragAndDrop
{
    /// <summary>
    /// Collection containing one or more <see cref="DragData"/> entries
    /// </summary>
    public sealed class DragDataCollection
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public DragDataCollection()
        {
            Collection = new List<DragData>();
            EntryType = string.Empty;
        }

        /// <summary>
        /// Construct from a list of <see cref="DragData"/> entries
        /// </summary>
        /// <param name="data">Data to populate the collection</param>
        public DragDataCollection(List<DragData> data)
            : this()
        {
            Collection.AddRange(data);
            if(data.Select(x => x.DataType).Distinct().Count() == 1)
            {
                EntryType = data.First().DataType;
            }
        }

        /// <summary>
        /// The type of entries in the collection.
        /// Should only be set if the entries are of the same type
        /// </summary>
        public string EntryType { get; set; }

        /// <summary>
        /// Collection containing the <see cref="DragData"/> entries
        /// </summary>
        public List<DragData> Collection { get; set; }
    }
}
