using System.Text.Json.Serialization;

namespace VaraniumSharp.WinUI.FilterModule
{
    /// <summary>
    /// Context used for System.Text.Json source generator serializing <see cref="FilterStorageWrapperModel"/>
    /// </summary>
    [JsonSerializable(typeof(FilterStorageWrapperModel))]
    public partial class FilterStorageWrapperModelJsonContext : JsonSerializerContext
    {}
}