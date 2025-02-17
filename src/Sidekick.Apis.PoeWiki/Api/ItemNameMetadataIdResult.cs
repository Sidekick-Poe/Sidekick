using System.Text.Json.Serialization;

namespace Sidekick.Apis.PoeWiki.Api;

public record ItemNameMetadataIdResult
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("metadata id")]
    public string? MetadataId { get; init; }
}
