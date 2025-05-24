using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Trade.Results;

public class Extended
{
    public string? Text { get; set; }

    public Mods? Mods { get; set; }

    public Hashes? Hashes { get; set; }

    [JsonPropertyName("dps")]
    public double? DamagePerSecondAtMax { get; set; }

    [JsonPropertyName("dps_aug")]
    public bool DamagePerSecondAugmented { get; set; }

    [JsonPropertyName("edps")]
    public double? ElementalDpsAtMax { get; set; }

    [JsonPropertyName("edps_aug")]
    public bool ElementalDpsAugmented { get; set; }

    [JsonPropertyName("pdps")]
    public double? PhysicalDpsAtMax { get; set; }

    [JsonPropertyName("pdps_aug")]
    public bool PhysicalDpsAugmented { get; set; }

    [JsonPropertyName("ar")]
    public int? ArmourAtMax { get; set; }

    [JsonPropertyName("ar_aug")]
    public bool ArmourAugmented { get; set; }

    [JsonPropertyName("ev")]
    public int? EvasionAtMax { get; set; }

    [JsonPropertyName("ev_aug")]
    public bool EvasionAugmented { get; set; }

    [JsonPropertyName("es")]
    public int? EnergyShieldAtMax { get; set; }

    [JsonPropertyName("es_aug")]
    public bool EnergyShieldAugmented { get; set; }

    [JsonPropertyName("base_defence_percentile")]
    public int? BaseDefencePercentile { get; set; }
}
