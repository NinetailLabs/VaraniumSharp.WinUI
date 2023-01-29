using System.Text.Json.Serialization;

namespace VaraniumSharp.WinUI.CustomShaping;

[JsonSerializable(typeof(CustomStorageWrapperModel))]
public partial class CustomStorageWrapperModelJsonContext : JsonSerializerContext
{}