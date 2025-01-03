using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal class ArmourFilters
    {
        [JsonPropertyName("ar")]
        public StatFilterValue? Armour { get; set; }

        [JsonPropertyName("es")]
        public StatFilterValue? EnergyShield { get; set; }

        [JsonPropertyName("ev")]
        public StatFilterValue? EvasionRating { get; set; }

        [JsonPropertyName("block")]
        public StatFilterValue? BlockChance { get; set; }
    }
}
