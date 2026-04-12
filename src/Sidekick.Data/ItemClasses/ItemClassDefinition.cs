using System.Text.Json.Serialization;
namespace Sidekick.Data.ItemClasses;

public class ItemClassDefinition
{
    public string? Id { get; init; }

    public ItemClass Type { get; init; }

    public string? Name { get; init; }

    [JsonIgnore]
    private static readonly ItemClass[] Equipment =
    [
        ItemClass.BodyArmour,
        ItemClass.Boots,
        ItemClass.Gloves,
        ItemClass.Helmet,
        ItemClass.Quiver,
        ItemClass.Shield,
        ItemClass.Focus,
        ItemClass.Buckler,
    ];

    public bool IsEquipment() => Equipment.Contains(Type);

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
    private static readonly ItemClass[] Accessories =
    [
        ItemClass.Amulet,
        ItemClass.Belt,
        ItemClass.Ring,
        ItemClass.Trinket,
    ];

    public bool IsAccessory() => Accessories.Contains(Type);

    [JsonIgnore]
    private static readonly ItemClass[] Jewels =
    [
        ItemClass.Jewel,
        ItemClass.AbyssJewel,
    ];

    public bool IsJewel() => Jewels.Contains(Type);

    [JsonIgnore]
    private static readonly ItemClass[] Flasks =
    [
        ItemClass.Flask,
        ItemClass.LifeFlask,
        ItemClass.ManaFlask,
        ItemClass.Tincture,
        ItemClass.Charms,
    ];

    public bool IsFlask() => Flasks.Contains(Type);

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
    ];

    public bool IsArea() => Areas.Contains(Type);

    [JsonIgnore]
    private static readonly ItemClass[] Gems =
    [
        ItemClass.ActiveSkillGem,
        ItemClass.SupportSkillGem,
    ];

    public bool IsGem() => Gems.Contains(Type);

    [JsonIgnore]
    private static readonly ItemClass[] WithStats =
    [
        ..Equipment,
        ..Weapons,
        ..Accessories,
        ..Flasks,

        ..Jewels,
        ..Areas,

        ItemClass.Idol,

        ItemClass.HeistEquipmentReward,
        ItemClass.HeistEquipmentUtility,
        ItemClass.HeistEquipmentWeapon,
        ItemClass.HeistEquipmentTool,

        ItemClass.SanctumRelic,
        ItemClass.ActiveSkillGem,
    ];

    public bool CanHaveStats() => WithStats.Contains(Type);
}
