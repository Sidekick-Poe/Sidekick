using System.Text.Json.Serialization;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Trade.Results
{
    public class Extended
    {
        public string? Text { get; set; }

        public Mods? Mods { get; set; }

        public Hashes? Hashes { get; set; }

        [JsonPropertyName("dps")]
        public double DamagePerSecond { get; set; }

        [JsonPropertyName("edps")]
        public double ElementalDps { get; set; }

        [JsonPropertyName("pdps")]
        public double PhysicalDps { get; set; }

        [JsonPropertyName("ar")]
        public int ArmourAtMax { get; set; }

        [JsonPropertyName("ar_aug")]
        public bool ArmourAugmented { get; set; }

        [JsonPropertyName("ev")]
        public int EvasionAtMax { get; set; }

        [JsonPropertyName("ev_aug")]
        public bool EvasionAugmented { get; set; }

        [JsonPropertyName("es")]
        public int EnergyShieldAtMax { get; set; }

        [JsonPropertyName("es_aug")]
        public bool EnergyShieldAugmented { get; set; }

        [JsonPropertyName("base_defence_percentile")]
        public int? BaseDefencePercentile { get; set; }

        [JsonPropertyName("physical_damage")]
        public DamageRange PhysicalDamage { get; set; } = new();

        [JsonPropertyName("elemental_damage")]
        public List<DamageRange> ElementalDamage { get; set; } = new();

        [JsonPropertyName("crit")]
        public double CriticalStrikeChance { get; set; }

        [JsonPropertyName("aps")]
        public double AttacksPerSecond { get; set; }
    }
}
