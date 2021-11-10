using System.Text.Json.Serialization;

namespace VaraniumSharp.WinUI.DragAndDrop
{
    /// <summary>
    /// Context used for System.Text.Json source generator for serializing <see cref="DragData"/>
    /// </summary>
    [JsonSerializable(typeof(DragData))]
    public partial class DragDataJsonContext : JsonSerializerContext
    { }
}
