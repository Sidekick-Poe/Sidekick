using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal class ArmourFilters
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
