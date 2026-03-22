using Sidekick.Common.Enums;
namespace Sidekick.Data.Items;

public enum ItemClass
{
    Unknown = 0,

    [EnumValue("currency")]
    [EnumValue("Currency", key: "Game")]
    [EnumValue("StackableCurrency", key: "Game")]
    Currency,

    [EnumValue("accessory.amulet")]
    [EnumValue("Amulet", key: "Game")]
    Amulet,

    [EnumValue("accessory.belt")]
    [EnumValue("Belt", key: "Game")]
    Belt,

    [EnumValue("accessory.ring")]
    [EnumValue("Ring", key: "Game")]
    Ring,

    [EnumValue("armour.chest")]
    [EnumValue("Body Armour", key: "Game")]
    BodyArmour,

    [EnumValue("armour.boots")]
    [EnumValue("Boots", key: "Game")]
    Boots,

    [EnumValue("armour.gloves")]
    [EnumValue("Gloves", key: "Game")]
    Gloves,

    [EnumValue("armour.helmet")]
    [EnumValue("Helmet", key: "Game")]
    Helmet,

    [EnumValue("armour.quiver")]
    [EnumValue("Quiver", key: "Game")]
    Quiver,

    [EnumValue("armour.shield")]
    [EnumValue("Shield", key: "Game")]
    Shield,

    [EnumValue("armour.focus")]
    [EnumValue("Focus", key: "Game")]
    Focus,

    [EnumValue("armour.buckler")]
    [EnumValue("Buckler", key: "Game")]
    Buckler,

    [EnumValue("card")]
    [EnumValue("DivinationCard", key: "Game")]
    DivinationCard,

    [EnumValue("currency.resonator")]
    [EnumValue("DelveSocketableCurrency", key: "Game")]
    [EnumValue("DelveStackableSocketableCurrency", key: "Game")]
    Resonator,

    [EnumValue("accessory.trinket")]
    [EnumValue("Trinket", key: "Game")]
    Trinket,

    [EnumValue("currency.heistobjective")]
    [EnumValue("HeistObjective", key: "Game")]
    HeistObjective,

    [EnumValue("currency.omen")]
    [EnumValue("Omen", key: "Game")]
    Omen,

    [EnumValue("currency.socketable")]
    [EnumValue("Socketable", key: "Game")]
    Socketable,

    [EnumValue("flask")]
    [EnumValue("Flask", key: "Game")]
    Flask,

    [EnumValue("flask.life")]
    [EnumValue("LifeFlask", key: "Game")]
    LifeFlask,

    [EnumValue("flask.mana")]
    [EnumValue("ManaFlask", key: "Game")]
    ManaFlask,

    [EnumValue("AnimalCharm", key: "Game")]
    Charms,

    [EnumValue("gem.activegem")]
    [EnumValue("Active Skill Gem", key: "Game")]
    ActiveGem,

    [EnumValue("gem.supportgem")]
    [EnumValue("Support Skill Gem", key: "Game")]
    SupportGem,

    UncutSkillGem,

    UncutSupportGem,

    UncutSpiritGem,

    [EnumValue("heistmission.blueprint")]
    [EnumValue("HeistBlueprint", key: "Game")]
    Blueprint,

    [EnumValue("heistmission.contract")]
    [EnumValue("HeistContract", key: "Game")]
    Contract,

    [EnumValue("heistequipment.heistreward")]
    [EnumValue("HeistEquipmentReward", key: "Game")]
    HeistReward,

    [EnumValue("heistequipment.heistutility")]
    [EnumValue("HeistEquipmentUtility", key: "Game")]
    HeistUtility,

    [EnumValue("heistequipment.heistweapon")]
    [EnumValue("HeistEquipmentWeapon", key: "Game")]
    HeistWeapon,

    [EnumValue("heistequipment.heisttool")]
    [EnumValue("HeistEquipmentTool", key: "Game")]
    HeistTool,

    [EnumValue("jewel")]
    [EnumValue("Jewel", key: "Game")]
    Jewel,

    [EnumValue("jewel.abyss")]
    [EnumValue("AbyssJewel", key: "Game")]
    AbyssJewel,

    [EnumValue("logbook")]
    [EnumValue("ExpeditionLogbook", key: "Game")]
    Logbook,

    [EnumValue("map.waystone")]
    [EnumValue("MapKey", key: "Game")]
    Waystone,

    [EnumValue("map.breachstone")]
    [EnumValue("Breachstone", key: "Game")]
    Breachstone,

    [EnumValue("map.barya")]
    Barya,

    [EnumValue("map.bosskey")]
    BossKey,

    [EnumValue("map.ultimatum")]
    Ultimatum,

    [EnumValue("map.tablet")]
    Tablet,

    [EnumValue("map.fragment")]
    [EnumValue("MapFragment", key: "Game")]
    MapFragment,

    [EnumValue("map")]
    [EnumValue("Map", key: "Game")]
    [EnumValue("InstanceLocalItem", key: "Game")]
    Map,

    [EnumValue("memoryline")]
    [EnumValue("MemoryLine", key: "Game")]
    MemoryLine,

    [EnumValue("weapon.bow")]
    [EnumValue("Bow", key: "Game")]
    Bow,

    [EnumValue("weapon.crossbow")]
    Crossbow,

    [EnumValue("weapon.claw")]
    [EnumValue("Claw", key: "Game")]
    Claw,

    [EnumValue("weapon.dagger")]
    [EnumValue("Dagger", key: "Game")]
    Dagger,

    [EnumValue("weapon.runedagger")]
    [EnumValue("Rune Dagger", key: "Game")]
    RuneDagger,

    [EnumValue("weapon.oneaxe")]
    [EnumValue("One Hand Axe", key: "Game")]
    OneHandAxe,

    [EnumValue("weapon.onemace")]
    [EnumValue("One Hand Mace", key: "Game")]
    OneHandMace,

    [EnumValue("weapon.onesword")]
    [EnumValue("One Hand Sword", key: "Game")]
    OneHandSword,

    [EnumValue("weapon.sceptre")]
    [EnumValue("Sceptre", key: "Game")]
    Sceptre,

    [EnumValue("weapon.staff")]
    [EnumValue("Staff", key: "Game")]
    Staff,

    [EnumValue("weapon.rod")]
    [EnumValue("FishingRod", key: "Game")]
    FishingRod,

    [EnumValue("weapon.talisman")]
    [EnumValue("VaultKey", key: "Game")]
    Talisman,

    [EnumValue("weapon.twoaxe")]
    [EnumValue("Two Hand Axe", key: "Game")]
    TwoHandAxe,

    [EnumValue("weapon.twomace")]
    [EnumValue("Two Hand Mace", key: "Game")]
    TwoHandMace,

    [EnumValue("weapon.twosword")]
    [EnumValue("Two Hand Sword", key: "Game")]
    TwoHandSword,

    [EnumValue("weapon.wand")]
    [EnumValue("Wand", key: "Game")]
    Wand,

    [EnumValue("weapon.warstaff")]
    [EnumValue("Warstaff", key: "Game")]
    Warstaff,

    [EnumValue("weapon.spear")]
    Spear,

    [EnumValue("tincture")]
    [EnumValue("Tincture", key: "Game")]
    Tincture,

    [EnumValue("corpse")]
    [EnumValue("ItemisedCorpse", key: "Game")]
    Corpse,

    [EnumValue("sanctum.relic")]
    [EnumValue("Relic", key: "Game")]
    [EnumValue("SanctumSpecialRelic", key: "Game")]
    [EnumValue("SmallRelic", key: "Game")]
    [EnumValue("MediumRelic", key: "Game")]
    [EnumValue("LargeRelic", key: "Game")]
    SanctumRelic,

    [EnumValue("sanctum.research")]
    [EnumValue("ItemisedSanctum", key: "Game")]
    SanctumResearch,

    [EnumValue("idol")]
    Idol,

    [EnumValue("graft")]
    Graft,

    [EnumValue("wombgift")]
    Wombgift,

    [EnumValue("currency.incubator")]
    [EnumValue("IncubatorStackable", key: "Game")]
    Incubator,
}
