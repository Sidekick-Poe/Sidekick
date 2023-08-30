using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Filters
{
    public class ArmorFilter
    {
        [JsonPropertyName("ar")]
        public SearchFilterValue? Armor { get; set; }

        [JsonPropertyName("es")]
        public SearchFilterValue? EnergyShield { get; set; }

        [JsonPropertyName("ev")]
        public SearchFilterValue? Evasion { get; set; }

        [JsonPropertyName("block")]
        public SearchFilterValue? Block { get; set; }
    }
}
