using System.Text.Json.Serialization;

namespace VaraniumSharp.WinUI.DragAndDrop
{
    /// <summary>
    /// Context used for System.Text.Json source generator for serializing <see cref="DragDataCollection"/>
    /// </summary>
    [JsonSerializable(typeof(DragDataCollection))]
    public partial class DragDataCollectionJsonContext : JsonSerializerContext
    { }
}
