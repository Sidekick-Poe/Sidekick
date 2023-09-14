using System.Text.Json.Serialization;

namespace Sidekick.Apis.PoeWiki.Api
{
    internal record ItemIdResult
    {
        [JsonPropertyName("item id")]
        public string? ItemId { get; init; }
    }
}
