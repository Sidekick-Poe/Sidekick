namespace Sidekick.Apis.Poe.Items;

public static class ItemClassConstants
{
    public static readonly ItemClass[] Gems =
    [
        ItemClass.ActiveGem,
        ItemClass.SupportGem,
        ItemClass.UncutSkillGem,
        ItemClass.UncutSupportGem,
        ItemClass.UncutSpiritGem,
    ];

    public static readonly ItemClass[] Equipment =
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

    public static readonly ItemClass[] Weapons =
    [
        ItemClass.Bow,
        ItemClass.Crossbow,
        ItemClass.Claw,
        ItemClass.Dagger,
        ItemClass.RuneDagger,
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

    public static readonly ItemClass[] Accessories =
    [
        ItemClass.Amulet,
        ItemClass.Belt,
        ItemClass.Ring,
        ItemClass.Trinket,
    ];

    public static readonly ItemClass[] Jewels =
    [
        ItemClass.Jewel,
        ItemClass.AbyssJewel,
    ];

    public static readonly ItemClass[] Flasks =
    [
        ItemClass.Flask,
        ItemClass.LifeFlask,
        ItemClass.ManaFlask,
        ItemClass.Tincture,
        ItemClass.Charms,
    ];

    public static readonly ItemClass[] Areas =
    [
        ItemClass.Blueprint,
        ItemClass.Contract,
        ItemClass.Logbook,
        ItemClass.Tablet,
        ItemClass.Map,
        ItemClass.Waystone,
        ItemClass.Barya,
        ItemClass.Ultimatum,
        ItemClass.SanctumResearch,
        ItemClass.MapFragment, // Necessary for itemised Atzoatl temples
    ];

    public static readonly ItemClass[] WithModifiers =
    [
        ..Equipment,
        ..Weapons,
        ..Accessories,
        ..Flasks,

        ..Jewels,
        ..Areas,

        ItemClass.Idol,

        ItemClass.HeistReward,
        ItemClass.HeistUtility,
        ItemClass.HeistWeapon,
        ItemClass.HeistTool,

        ItemClass.SanctumRelic,
        ItemClass.SanctumResearch,

        ItemClass.Graft,
    ];
}
