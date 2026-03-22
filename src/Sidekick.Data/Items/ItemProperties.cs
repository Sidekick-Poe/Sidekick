using System.Text.Json.Serialization;
namespace Sidekick.Data.Items;

public class ItemProperties
{
    public ItemPropertyValues? Armour { get; init; }

    public ItemPropertyValues? EnergyShield { get; init; }

    public ItemPropertyValues? Evasion { get; init; }

    public ItemPropertyValues? PhysicalDamage { get; init; }

    public int? Block { get; init; }

    public double? AttacksPerSecond { get; init; }

    public double? CriticalHitChance { get; init; }
}