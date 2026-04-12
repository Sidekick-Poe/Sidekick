using System.Text.Json.Serialization;
namespace Sidekick.Data.Builder.Repoe.Models.Items;

public class RepoeItemClass
{
    [JsonPropertyName("category")]
    public string? Category { get; init; }

    [JsonPropertyName("category_id")]
    public string? CategoryId { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("influence_tags")]
    public List<string>? InfluenceTags { get; init; }
}
