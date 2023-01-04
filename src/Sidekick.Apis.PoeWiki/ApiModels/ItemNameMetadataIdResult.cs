using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sidekick.Apis.PoeWiki.JsonConverters;

namespace Sidekick.Apis.PoeWiki.ApiModels
{
    public class ItemNameMetadataIdResult
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("metadata id")]
        public string MetadataId { get; set; }
    }
}
