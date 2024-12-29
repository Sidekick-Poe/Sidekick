using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal class MapFilters
    {
        [JsonPropertyName("map_iiq")]
        public StatFilterValue? ItemQuantity { get; set; }

        [JsonPropertyName("map_iir")]
        public StatFilterValue? ItemRarity { get; set; }

        [JsonPropertyName("area_level")]
        public StatFilterValue? AreaLevel { get; set; }

        [JsonPropertyName("map_tier")]
        public StatFilterValue? MapTier { get; set; }

        [JsonPropertyName("map_packsize")]
        public StatFilterValue? MonsterPackSize { get; set; }

        [JsonPropertyName("map_blighted")]
        public SearchFilterOption? Blighted { get; set; }

        [JsonPropertyName("map_uberblighted")]
        public SearchFilterOption? BlightRavavaged { get; set; }

        [JsonPropertyName("map_elder")]
        public SearchFilterOption? Elder { get; set; }

        [JsonPropertyName("map_shaped")]
        public SearchFilterOption? Shaped { get; set; }
    }
}
