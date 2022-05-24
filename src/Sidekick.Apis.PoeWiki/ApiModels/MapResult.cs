using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sidekick.Apis.PoeWiki.JsonConverters;

namespace Sidekick.Apis.PoeWiki.ApiModels
{
    public class MapResult
    {
        public string Name { get; set; }

        [JsonPropertyName("area id")]
        public string AreaId { get; set; }

        [JsonPropertyName("boss monster ids")]
        [JsonConverter(typeof(StringListToListOfStringsJsonConverter))]
        public List<string> BossMonsterIds { get; set; } = new();
    }
}
