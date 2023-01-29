using System.Text.Json.Serialization;

namespace VaraniumSharp.WinUI.CustomShaping;

/// <summary>
/// Context used for System.Text.Json source generator serializing <see cref="CustomStorageWrapperModel"/>
/// </summary>
[JsonSerializable(typeof(CustomStorageWrapperModel))]
public partial class CustomStorageWrapperModelJsonContext : JsonSerializerContext
{}