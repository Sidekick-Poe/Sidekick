using System.Text.Json.Serialization;
namespace Sidekick.Data.Builder.Repoe.Models.Items;

public class RepoeBaseItemProperties
{
    [JsonPropertyName("armour")]
    public RepoeBaseItemProperty? Armour { get; init; }

    [JsonPropertyName("energy_shield")]
    public RepoeBaseItemProperty? EnergyShield { get; init; }

    [JsonPropertyName("evasion")]
    public RepoeBaseItemProperty? Evasion { get; init; }

    [JsonPropertyName("ward")]
    public RepoeBaseItemProperty? Ward { get; init; }

    [JsonPropertyName("block")]
    public int? Block { get; init; }

    [JsonPropertyName("attack_time")]
    public int? AttackMilliseconds { get; init; }

    [JsonPropertyName("critical_strike_chance")]
    public int? CriticalHitChance { get; init; }

    [JsonPropertyName("physical_damage_max")]
    public int? PhysicalDamageMax { get; init; }

    [JsonPropertyName("physical_damage_min")]
    public int? PhysicalDamageMin { get; init; }

    [JsonIgnore]
    public bool HasValues
    {
        get
        {
            return Armour != null
                   || EnergyShield != null
                   || Evasion != null
                   || Block.HasValue
                   || AttackMilliseconds.HasValue
                   || CriticalHitChance.HasValue
                   || PhysicalDamageMax.HasValue
                   || PhysicalDamageMin.HasValue;
        }
    }
}
