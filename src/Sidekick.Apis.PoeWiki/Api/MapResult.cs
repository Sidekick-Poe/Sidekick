using System.Text.Json.Serialization;
using Sidekick.Apis.PoeWiki.JsonConverters;

namespace Sidekick.Apis.PoeWiki.Api;

internal record MapResult
{
    public string? Name { get; init; }

    [JsonPropertyName("area id")]
    public string? AreaId { get; init; }

    [JsonPropertyName("boss monster ids")]
    [JsonConverter(typeof(StringListToListOfStringsJsonConverter))]
    public List<string> BossMonsterIds { get; init; } = new();

    [JsonPropertyName("area type tags")]
    [JsonConverter(typeof(StringListToListOfStringsJsonConverter))]
    public List<string> AreaTypeTags { get; init; } = new();
}
