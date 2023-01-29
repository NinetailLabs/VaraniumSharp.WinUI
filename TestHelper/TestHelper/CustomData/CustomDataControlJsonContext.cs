using System.Text.Json.Serialization;

namespace TestHelper.CustomData;

[JsonSerializable(typeof(CustomDataControlJson))]
public partial class CustomDataControlJsonContext : JsonSerializerContext
{}