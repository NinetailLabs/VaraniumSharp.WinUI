using System.Text.Json.Serialization;

namespace VaraniumSharp.WinUI.TabViewHelpers
{
    /// <summary>
    /// Context used for System.Text.Json source generator for serializing <see cref="TabsContainerModel"/>
    /// </summary>
    [JsonSerializable(typeof(TabsContainerModel))]
    public partial class TabsContainerJsonContext : JsonSerializerContext
    {
    }
}
