using System.Text.Json.Serialization;

namespace Sidekick.Apis.PoeWiki.ApiModels
{
    public class ItemIdResult
    {
        [JsonPropertyName("item id")]
        public string? ItemId { get; set; }
    }
}
