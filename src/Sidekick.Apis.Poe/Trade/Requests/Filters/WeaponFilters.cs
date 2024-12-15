using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal class WeaponFilters
    {
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
        public SearchFilterValue? PhysicalDamage { get; set; }
    }
}
