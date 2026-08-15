using System.Text.Json.Serialization;

namespace Sidekick.Game.BaseItems;

public class BaseItemProperties
{
    [JsonPropertyName("ar")]
    public BaseItemPropertyValues? Armour { get; init; }

    [JsonPropertyName("es")]
    public BaseItemPropertyValues? EnergyShield { get; init; }

    [JsonPropertyName("ev")]
    public BaseItemPropertyValues? Evasion { get; init; }

    [JsonPropertyName("ward")]
    public BaseItemPropertyValues? Ward { get; init; }

    [JsonPropertyName("pdmg")]
    public BaseItemPropertyValues? PhysicalDamage { get; init; }

    [JsonPropertyName("block")]
    public int? Block { get; init; }

    [JsonPropertyName("aps")]
    public double? AttacksPerSecond { get; init; }

    [JsonPropertyName("crit")]
    public double? CriticalHitChance { get; init; }

    [JsonIgnore]
    public bool HasAnyValues => Armour != null || EnergyShield != null || Evasion != null || Ward != null || PhysicalDamage != null || Block != null || AttacksPerSecond != null || CriticalHitChance != null;
}