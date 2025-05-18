using System.Text.Json.Serialization;

namespace Sidekick.Apis.PoeWiki.Api;

internal record StatTextResult
{
    [JsonPropertyName("item id")]
    public string? ItemId { get; init; }

    [JsonPropertyName("stat text")]
    public string? StatText { get; init; }
}
