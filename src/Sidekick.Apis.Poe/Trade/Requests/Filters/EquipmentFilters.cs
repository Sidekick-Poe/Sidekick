using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters;

public class EquipmentFilters
{
    [JsonPropertyName("ar")]
    public StatFilterValue? Armor { get; set; }

    [JsonPropertyName("es")]
    public StatFilterValue? EnergyShield { get; set; }

    [JsonPropertyName("ev")]
    public StatFilterValue? Evasion { get; set; }

    [JsonPropertyName("block")]
    public StatFilterValue? Block { get; set; }

    [JsonPropertyName("crit")]
    public StatFilterValue? CriticalStrikeChance { get; set; }

    [JsonPropertyName("aps")]
    public StatFilterValue? AttacksPerSecond { get; set; }

    [JsonPropertyName("dps")]
    public StatFilterValue? DamagePerSecond { get; set; }

    [JsonPropertyName("edps")]
    public StatFilterValue? ElementalDps { get; set; }

    [JsonPropertyName("pdps")]
    public StatFilterValue? PhysicalDps { get; set; }

    [JsonPropertyName("damage")]
    public StatFilterValue? Damage { get; set; }
}
