using Sidekick.Common.Enums;
namespace Sidekick.Apis.Poe.Items;

public enum ItemClass
{
    Unknown = 0,

    [EnumValue("accessory.amulet")]
    Amulet,

    [EnumValue("accessory.belt")]
    Belt,

    [EnumValue("accessory.ring")]
    Ring,

    [EnumValue("accessory.trinket")]
    Trinket,

    [EnumValue("armour.chest")]
    BodyArmour,

    [EnumValue("armour.boots")]
    Boots,

    [EnumValue("armour.gloves")]
    Gloves,

    [EnumValue("armour.helmet")]
    Helmet,

    [EnumValue("armour.quiver")]
    Quiver,

    [EnumValue("armour.shield")]
    Shield,

    [EnumValue("armour.focus")]
    Focus,

    [EnumValue("armour.buckler")]
    Buckler,

    [EnumValue("card")]
    DivinationCard,

    [EnumValue("currency")]
    Currency,

    [EnumValue("currency.resonator")]
    Resonator,

    [EnumValue("currency.heistobjective")]
    HeistObjective,

    [EnumValue("currency.omen")]
    Omen,

    [EnumValue("currency.socketable")]
    Socketable,

    [EnumValue("flask")]
    Flask,

    [EnumValue("flask.life")]
    LifeFlask,

    [EnumValue("flask.mana")]
    ManaFlask,

    [EnumValue("gem.activegem")]
    ActiveGem,

    [EnumValue("gem.supportgem")]
    SupportGem,

    UncutSkillGem,

    UncutSupportGem,

    UncutSpiritGem,

    [EnumValue("heistmission.blueprint")]
    Blueprint,

    [EnumValue("heistmission.contract")]
    Contract,

    [EnumValue("heistequipment.heistreward")]
    HeistReward,

    [EnumValue("heistequipment.heistutility")]
    HeistUtility,

    [EnumValue("heistequipment.heistweapon")]
    HeistWeapon,

    [EnumValue("heistequipment.heisttool")]
    HeistTool,

    [EnumValue("jewel")]
    Jewel,

    [EnumValue("jewel.abyss")]
    AbyssJewel,

    [EnumValue("logbook")]
    Logbook,

    [EnumValue("map.waystone")]
    Waystone,

    [EnumValue("map.breachstone")]
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
    MapFragment,

    [EnumValue("map")]
    Map,

    [EnumValue("memoryline")]
    MemoryLine,

    [EnumValue("weapon.bow")]
    Bow,

    [EnumValue("weapon.crossbow")]
    Crossbow,

    [EnumValue("weapon.claw")]
    Claw,

    [EnumValue("weapon.dagger")]
    Dagger,

    [EnumValue("weapon.runedagger")]
    RuneDagger,

    [EnumValue("weapon.oneaxe")]
    OneHandAxe,

    [EnumValue("weapon.onemace")]
    OneHandMace,

    [EnumValue("weapon.onesword")]
    OneHandSword,

    [EnumValue("weapon.sceptre")]
    Sceptre,

    [EnumValue("weapon.staff")]
    Staff,

    [EnumValue("weapon.rod")]
    FishingRod,

    [EnumValue("weapon.twoaxe")]
    TwoHandAxe,

    [EnumValue("weapon.twomace")]
    TwoHandMace,

    [EnumValue("weapon.twosword")]
    TwoHandSword,

    [EnumValue("weapon.wand")]
    Wand,

    [EnumValue("weapon.warstaff")]
    Warstaff,

    [EnumValue("weapon.spear")]
    Spear,

    [EnumValue("tincture")]
    Tincture,

    [EnumValue("corpse")]
    Corpse,

    [EnumValue("sanctum.relic")]
    SanctumRelic,

    [EnumValue("sanctum.research")]
    SanctumResearch,

    [EnumValue("idol")]
    Idol,
}
