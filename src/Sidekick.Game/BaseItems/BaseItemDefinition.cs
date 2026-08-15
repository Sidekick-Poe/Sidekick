using System.Text.Json.Serialization;
using Sidekick.Game.ItemClasses;

namespace Sidekick.Game.BaseItems;

public class BaseItemDefinition
{
    public required string Id { get; set; }

    public string? ItemClassId { get; init; }

    public string? Name { get; init; }

    [JsonPropertyName("prop")]
    public BaseItemProperties? Properties { get; set; }

    [JsonPropertyName("req")]
    public BaseItemRequirements? Requirements { get; set; }

    [JsonIgnore]
    public ItemClassDefinition? ItemClass { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Name} ({ItemClassId})";
    }
}