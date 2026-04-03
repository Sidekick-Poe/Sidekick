using Sidekick.Data.ItemDefinitions;
namespace Sidekick.Data.Items;

public static class ItemClassExtensions
{
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

    public static bool IsEquipment(this ItemClass value) => Equipment.Contains(value);

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

    public static bool IsWeapon(this ItemClass value) => Weapons.Contains(value);

    private static readonly ItemClass[] Accessories =
    [
        ItemClass.Amulet,
        ItemClass.Belt,
        ItemClass.Ring,
        ItemClass.Trinket,
    ];

    public static bool IsAccessory(this ItemClass value) => Accessories.Contains(value);

    private static readonly ItemClass[] Jewels =
    [
        ItemClass.Jewel,
        ItemClass.AbyssJewel,
    ];

    public static bool IsJewel(this ItemClass value) => Jewels.Contains(value);

    private static readonly ItemClass[] Flasks =
    [
        ItemClass.Flask,
        ItemClass.LifeFlask,
        ItemClass.ManaFlask,
        ItemClass.Tincture,
        ItemClass.Charms,
    ];

    public static bool IsFlask(this ItemClass value) => Flasks.Contains(value);

    private static readonly ItemClass[] Areas =
    [
        ItemClass.HeistBlueprint,
        ItemClass.HeistContract,
        ItemClass.ExpeditionLogbook,
        ItemClass.Tablet,
        ItemClass.Map,
        ItemClass.Barya,
        ItemClass.Ultimatum,
    ];

    public static bool IsArea(this ItemClass value) => Areas.Contains(value);

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

    public static bool CanHaveStats(this ItemClass value) => WithStats.Contains(value);
}
