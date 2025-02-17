using System.Text.Json.Serialization;
using Sidekick.Common.Converters;

namespace Sidekick.Apis.PoeWiki.Api;

internal record ItemResult
{
    public string? Name { get; init; }

    [JsonPropertyName("description")]
    [JsonConverter(typeof(RemoveWikiTagsConverter))]
    public string? Description { get; init; }

    [JsonPropertyName("flavour text")]
    public string? FlavourText { get; init; }
}
