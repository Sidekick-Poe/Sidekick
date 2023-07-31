using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sidekick.Apis.PoeWiki.JsonConverters;

namespace Sidekick.Apis.PoeWiki.ApiModels
{
    public class ItemIdResult
    {
        [JsonPropertyName("item id")]
        public string ItemId { get; set; }
    }
}
