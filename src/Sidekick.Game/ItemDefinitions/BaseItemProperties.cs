using System.Text.Json.Serialization;

namespace Sidekick.Game.ItemDefinitions;

public class BaseItemProperties
{
    public BaseItemPropertyValues? Armour { get; init; }

    public BaseItemPropertyValues? EnergyShield { get; init; }

    public BaseItemPropertyValues? Evasion { get; init; }

    public BaseItemPropertyValues? Ward { get; init; }

    public BaseItemPropertyValues? PhysicalDamage { get; init; }

    public int? Block { get; init; }

    public double? AttacksPerSecond { get; init; }

    public double? CriticalHitChance { get; init; }

    [JsonIgnore]
    public bool HasAnyValues => Armour != null || EnergyShield != null || Evasion != null || Ward != null || PhysicalDamage != null || Block != null || AttacksPerSecond != null || CriticalHitChance != null;
}
