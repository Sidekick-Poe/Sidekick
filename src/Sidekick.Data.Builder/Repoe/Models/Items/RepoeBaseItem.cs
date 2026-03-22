using System.Text.Json.Serialization;
namespace Sidekick.Data.Builder.Repoe.Models.Items;

public class RepoeBaseItem
{
    [JsonPropertyName("item_class")]
    public string? ItemClass { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("properties")]
    public RepoeBaseItemProperties? Properties { get; init; }

    [JsonPropertyName("requirements")]
    public RepoeBaseItemRequirements? Requirements { get; init; }

    [JsonPropertyName("implicits")]
    public List<string>? ImplicitModifiers { get; init; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; init; }
}