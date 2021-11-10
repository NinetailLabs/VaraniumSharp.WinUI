using System.Text.Json.Serialization;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Context used for System.Text.Json source generator serializing <see cref="SortStorageWrapperModel"/>
    /// </summary>
    [JsonSerializable(typeof(SortStorageWrapperModel))]
    public partial class SortStorageWrapperModelJsonContext : JsonSerializerContext
    {
    }
}
