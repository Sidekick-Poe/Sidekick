using System.Text.Json.Serialization;

namespace Sidekick.Business.Apis.Poe.Trade.Search.Filters
{
    public class MapFilter
    {
        [JsonPropertyName("map_iiq")]
        public SearchFilterValue MapIiq { get; set; }

        [JsonPropertyName("map_iir")]
        public SearchFilterValue MapIir { get; set; }

        [JsonPropertyName("map_tier")]
        public SearchFilterValue MapTier { get; set; }

        [JsonPropertyName("map_packsize")]
        public SearchFilterValue MapPacksize { get; set; }

        [JsonPropertyName("map_blighted")]
        public SearchFilterOption Blighted { get; set; }

        [JsonPropertyName("map_elder")]
        public SearchFilterOption Elder { get; set; }

        [JsonPropertyName("map_shaped")]
        public SearchFilterOption Shaped { get; set; }
    }
}
