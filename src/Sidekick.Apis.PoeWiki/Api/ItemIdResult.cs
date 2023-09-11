using System.Text.Json.Serialization;

namespace Sidekick.Apis.PoeWiki.Api
{
    public record ItemIdResult
    {
        [JsonPropertyName("item id")]
        public string? ItemId { get; init; }
    }
}
