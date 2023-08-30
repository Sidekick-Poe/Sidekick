using System.Text.Json.Serialization;

namespace Sidekick.Apis.PoeWiki.ApiModels
{
    public class BossResult
    {
        public string? Name { get; set; }

        [JsonPropertyName("metadata id")]
        public string? MetadataId { get; set; }
    }
}
