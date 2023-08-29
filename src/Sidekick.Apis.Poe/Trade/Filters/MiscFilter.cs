using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Filters
{
    public class MiscFilter
    {
        public SearchFilterValue? Quality { get; set; }

        [JsonPropertyName("gem_level")]
        public SearchFilterValue? GemLevel { get; set; }

        [JsonPropertyName("ilvl")]
        public SearchFilterValue? ItemLevel { get; set; }

        [JsonPropertyName("talisman_art")]
        public SearchFilterOption? TalismanArt { get; set; }

        [JsonPropertyName("alternate_art")]
        public SearchFilterOption? AlternateArt { get; set; }

        public SearchFilterOption? Identified { get; set; }

        public SearchFilterOption? Corrupted { get; set; }

        [JsonPropertyName("scourge_tier")]
        public SearchFilterValue? Scourged { get; set; }

        public SearchFilterOption? Crafted { get; set; }

        public SearchFilterOption? Enchanted { get; set; }

        public SearchFilterOption? Veiled { get; set; }

        public SearchFilterOption? Mirrored { get; set; }

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

        [JsonPropertyName("synthesised_item")]
        public SearchFilterOption? SynthesisedItem { get; set; }
    }
}
