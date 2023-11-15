using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal class MiscFilter
    {
        public SearchFilterValue? Quality { get; set; }

        [JsonPropertyName("gem_level")]
        public SearchFilterValue? GemLevel { get; set; }

        [JsonPropertyName("gem_alternate_quality")]
        public SearchFilterOption? GemQualityType { get; set; }

        [JsonPropertyName("ilvl")]
        public SearchFilterValue? ItemLevel { get; set; }

        public SearchFilterOption? Corrupted { get; set; }

        [JsonPropertyName("scourge_tier")]
        public SearchFilterValue? Scourged { get; set; }

        [JsonPropertyName("elder_item")]
        public SearchFilterOption? ElderItem { get; set; }

        [JsonPropertyName("hunter_item")]
        public SearchFilterOption? HunterItem { get; set; }

        [JsonPropertyName("shaper_item")]
        public SearchFilterOption? ShaperItem { get; set; }

        [JsonPropertyName("warlord_item")]
        public SearchFilterOption? WarlordItem { get; set; }

        [JsonPropertyName("crusader_item")]
        public SearchFilterOption? CrusaderItem { get; set; }

        [JsonPropertyName("redeemer_item")]
        public SearchFilterOption? RedeemerItem { get; set; }
    }
}
