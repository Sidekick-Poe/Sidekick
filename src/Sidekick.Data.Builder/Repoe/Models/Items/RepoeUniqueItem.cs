using System.Text.Json.Serialization;
namespace Sidekick.Data.Builder.Repoe.Models.Items;

public class RepoeUniqueItem
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("item_class")]
    public string? ItemClass { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("visual_identity")]
    public RepoeUniqueItemVisual? Visual { get; init; }
}