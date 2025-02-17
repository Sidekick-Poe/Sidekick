using System.Text.Json.Serialization;

namespace Sidekick.Apis.PoeWiki.Api;

internal record BossResult
{
    public string? Name { get; init; }

    [JsonPropertyName("metadata id")]
    public string? MetadataId { get; init; }
}
