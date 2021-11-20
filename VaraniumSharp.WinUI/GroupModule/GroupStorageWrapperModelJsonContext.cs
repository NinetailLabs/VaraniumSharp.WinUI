using System.Text.Json.Serialization;

namespace VaraniumSharp.WinUI.GroupModule
{
    /// <summary>
    /// Context used for System.Text.Json source generator serializing <see cref="GroupStorageWrapperModel"/>
    /// </summary>
    [JsonSerializable(typeof(GroupStorageWrapperModel))]
    public partial class GroupStorageWrapperModelJsonContext : JsonSerializerContext
    {}
}