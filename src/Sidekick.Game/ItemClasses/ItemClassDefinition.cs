using System.Text.Json.Serialization;
namespace Sidekick.Game.ItemClasses;

public class ItemClassDefinition
{
    public string? Id { get; init; }

    public ItemClass Type { get; init; }

    public string? Name { get; init; }

    [JsonIgnore]
    private static readonly ItemClass[] Weapons =
    [
        ItemClass.Bow,
        ItemClass.Crossbow,
        ItemClass.Claw,
        ItemClass.Dagger,
        ItemClass.OneHandAxe,
        ItemClass.OneHandMace,
        ItemClass.OneHandSword,
        ItemClass.Sceptre,
        ItemClass.Staff,
        ItemClass.FishingRod,
        ItemClass.Talisman,
        ItemClass.TwoHandAxe,
        ItemClass.TwoHandMace,
        ItemClass.TwoHandSword,
        ItemClass.Wand,
        ItemClass.Warstaff,
        ItemClass.Spear,
    ];

    public bool IsWeapon() => Weapons.Contains(Type);

    [JsonIgnore]
    private static readonly ItemClass[] Areas =
    [
        ItemClass.HeistBlueprint,
        ItemClass.HeistContract,
        ItemClass.ExpeditionLogbook,
        ItemClass.Tablet,
        ItemClass.Map,
        ItemClass.Barya,
        ItemClass.Ultimatum,
        ItemClass.SanctumResearch,
        ItemClass.Chart,
    ];

    public bool IsArea() => Areas.Contains(Type);

    [JsonIgnore]
    private static readonly ItemClass[] Gems =
    [
        ItemClass.ActiveSkillGem,
        ItemClass.SupportSkillGem,
    ];

    public bool IsGem() => Gems.Contains(Type);

    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}
