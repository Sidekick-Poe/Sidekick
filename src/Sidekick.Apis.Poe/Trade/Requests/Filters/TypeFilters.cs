using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal class TypeFilters
    {
        public SearchFilterOption? Category { get; set; }

        public SearchFilterOption? Rarity { get; set; }

        [JsonPropertyName("ilvl")]
        public SearchFilterValue? ItemLevel { get; set; }
    }
}
