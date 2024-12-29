using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal class ArmourFilters
    {
        [JsonPropertyName("ar")]
        public StatFilterValue? Armor { get; set; }

        [JsonPropertyName("es")]
        public StatFilterValue? EnergyShield { get; set; }

        [JsonPropertyName("ev")]
        public StatFilterValue? Evasion { get; set; }

        [JsonPropertyName("block")]
        public StatFilterValue? Block { get; set; }
    }
}
