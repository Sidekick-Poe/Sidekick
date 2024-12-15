using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal class EquipmentFilters
    {
        [JsonPropertyName("ar")]
        public SearchFilterValue? Armor { get; set; }

        [JsonPropertyName("es")]
        public SearchFilterValue? EnergyShield { get; set; }

        [JsonPropertyName("ev")]
        public SearchFilterValue? Evasion { get; set; }

        [JsonPropertyName("block")]
        public SearchFilterValue? Block { get; set; }

        [JsonPropertyName("crit")]
        public SearchFilterValue? CriticalStrikeChance { get; set; }

        [JsonPropertyName("aps")]
        public SearchFilterValue? AttacksPerSecond { get; set; }

        [JsonPropertyName("dps")]
        public SearchFilterValue? DamagePerSecond { get; set; }

        [JsonPropertyName("edps")]
        public SearchFilterValue? ElementalDps { get; set; }

        [JsonPropertyName("pdps")]
        public SearchFilterValue? PhysicalDps { get; set; }

        [JsonPropertyName("damage")]
        public SearchFilterValue? Damage { get; set; }
    }
}
