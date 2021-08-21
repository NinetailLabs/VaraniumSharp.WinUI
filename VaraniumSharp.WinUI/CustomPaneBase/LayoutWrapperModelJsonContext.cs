using System.Text.Json.Serialization;

namespace VaraniumSharp.WinUI.CustomPaneBase
{
    /// <summary>
    /// Context used for System.Text.Json source generator serializing <see cref="LayoutWrapperModel"/>
    /// </summary>
    [JsonSerializable(typeof(LayoutWrapperModel))]
    public partial class LayoutWrapperModelJsonContext : JsonSerializerContext
    {}
}