using Sidekick.Data.ItemDefinitions;
namespace Sidekick.Data.ItemClasses;

public enum ItemClass
{
    Unknown = 0,

    [ItemClassGameId(GameType.PathOfExile1, "RemovedItem")]
    RemovedItem,

    [ItemClassTradeId(GameType.PathOfExile1, "accessory.amulet")]
    [ItemClassTradeId(GameType.PathOfExile2, "accessory.amulet")]
    [ItemClassGameId(GameType.PathOfExile1, "Amulet")]
    [ItemClassGameId(GameType.PathOfExile2, "Amulet")]
    Amulet,

    [ItemClassTradeId(GameType.PathOfExile1, "accessory.belt")]
    [ItemClassTradeId(GameType.PathOfExile2, "accessory.belt")]
    [ItemClassGameId(GameType.PathOfExile1, "Belt")]
    [ItemClassGameId(GameType.PathOfExile2, "Belt")]
    Belt,

    [ItemClassTradeId(GameType.PathOfExile1, "accessory.ring")]
    [ItemClassTradeId(GameType.PathOfExile2, "accessory.ring")]
    [ItemClassGameId(GameType.PathOfExile1, "Ring")]
    [ItemClassGameId(GameType.PathOfExile2, "Ring")]
    Ring,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.chest")]
    [ItemClassTradeId(GameType.PathOfExile2, "armour.chest")]
    [ItemClassGameId(GameType.PathOfExile1, "Body Armour")]
    [ItemClassGameId(GameType.PathOfExile2, "Body Armour")]
    BodyArmour,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.boots")]
    [ItemClassTradeId(GameType.PathOfExile2, "armour.boots")]
    [ItemClassGameId(GameType.PathOfExile1, "Boots")]
    [ItemClassGameId(GameType.PathOfExile2, "Boots")]
    Boots,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.gloves")]
    [ItemClassTradeId(GameType.PathOfExile2, "armour.gloves")]
    [ItemClassGameId(GameType.PathOfExile1, "Gloves")]
    [ItemClassGameId(GameType.PathOfExile2, "Gloves")]
    Gloves,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.helmet")]
    [ItemClassTradeId(GameType.PathOfExile2, "armour.helmet")]
    [ItemClassGameId(GameType.PathOfExile1, "Helmet")]
    [ItemClassGameId(GameType.PathOfExile2, "Helmet")]
    Helmet,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.quiver")]
    [ItemClassTradeId(GameType.PathOfExile2, "armour.quiver")]
    [ItemClassGameId(GameType.PathOfExile1, "Quiver")]
    [ItemClassGameId(GameType.PathOfExile2, "Quiver")]
    Quiver,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.shield")]
    [ItemClassTradeId(GameType.PathOfExile2, "armour.shield")]
    [ItemClassGameId(GameType.PathOfExile1, "Shield")]
    [ItemClassGameId(GameType.PathOfExile2, "Shield")]
    Shield,

    [ItemClassTradeId(GameType.PathOfExile2, "armour.focus")]
    [ItemClassGameId(GameType.PathOfExile2, "Focus")]
    Focus,

    [ItemClassTradeId(GameType.PathOfExile2, "armour.buckler")]
    [ItemClassGameId(GameType.PathOfExile2, "Buckler")]
    Buckler,

    [ItemClassTradeId(GameType.PathOfExile1, "accessory.trinket")]
    [ItemClassGameId(GameType.PathOfExile1, "Trinket")]
    Trinket,

    [ItemClassTradeId(GameType.PathOfExile1, "flask")]
    [ItemClassTradeId(GameType.PathOfExile2, "flask")]
    [ItemClassGameId(GameType.PathOfExile1, "Flask")]
    [ItemClassGameId(GameType.PathOfExile1, "UtilityFlask")]
    [ItemClassGameId(GameType.PathOfExile1, "HybridFlask")]
    [ItemClassGameId(GameType.PathOfExile1, "LifeFlask")]
    [ItemClassGameId(GameType.PathOfExile1, "ManaFlask")]
    Flask,

    [ItemClassTradeId(GameType.PathOfExile2, "flask.life")]
    [ItemClassGameId(GameType.PathOfExile2, "LifeFlask")]
    LifeFlask,

    [ItemClassTradeId(GameType.PathOfExile2, "flask.mana")]
    [ItemClassGameId(GameType.PathOfExile2, "ManaFlask")]
    ManaFlask,

    [ItemClassGameId(GameType.PathOfExile1, "AnimalCharm")]
    [ItemClassGameId(GameType.PathOfExile2, "UtilityFlask")]
    Charms,

    [ItemClassTradeId(GameType.PathOfExile1, "jewel")]
    [ItemClassTradeId(GameType.PathOfExile2, "jewel")]
    [ItemClassGameId(GameType.PathOfExile1, "Jewel")]
    [ItemClassGameId(GameType.PathOfExile2, "Jewel")]
    Jewel,

    [ItemClassTradeId(GameType.PathOfExile1, "jewel.abyss")]
    [ItemClassGameId(GameType.PathOfExile1, "AbyssJewel")]
    AbyssJewel,

    [ItemClassTradeId(GameType.PathOfExile2, "map.barya")]
    Barya,

    [ItemClassTradeId(GameType.PathOfExile2, "map.bosskey")]
    BossKey,

    [ItemClassTradeId(GameType.PathOfExile2, "map.ultimatum")]
    Ultimatum,

    [ItemClassTradeId(GameType.PathOfExile2, "map.tablet")]
    [ItemClassGameId(GameType.PathOfExile2, "TowerAugmentation")]
    Tablet,

    [ItemClassTradeId(GameType.PathOfExile1, "map")]
    [ItemClassTradeId(GameType.PathOfExile2, "map.waystone")]
    [ItemClassGameId(GameType.PathOfExile1, "Map")]
    [ItemClassGameId(GameType.PathOfExile1, "MapKey")]
    [ItemClassGameId(GameType.PathOfExile1, "InstanceLocalItem")]
    [ItemClassGameId(GameType.PathOfExile2, "Map")]
    [ItemClassGameId(GameType.PathOfExile2, "MapKey")]
    Map,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.bow")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.bow")]
    [ItemClassGameId(GameType.PathOfExile1, "Bow")]
    [ItemClassGameId(GameType.PathOfExile2, "Bow")]
    Bow,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.crossbow")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.crossbow")]
    [ItemClassGameId(GameType.PathOfExile2, "Crossbow")]
    Crossbow,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.claw")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.claw")]
    [ItemClassGameId(GameType.PathOfExile1, "Claw")]
    [ItemClassGameId(GameType.PathOfExile2, "Claw")]
    Claw,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.dagger")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.dagger")]
    [ItemClassGameId(GameType.PathOfExile1, "Dagger")]
    [ItemClassGameId(GameType.PathOfExile1, "Rune Dagger")]
    [ItemClassGameId(GameType.PathOfExile2, "Dagger")]
    Dagger,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.oneaxe")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.oneaxe")]
    [ItemClassGameId(GameType.PathOfExile1, "One Hand Axe")]
    [ItemClassGameId(GameType.PathOfExile2, "One Hand Axe")]
    OneHandAxe,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.onemace")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.onemace")]
    [ItemClassGameId(GameType.PathOfExile1, "One Hand Mace")]
    [ItemClassGameId(GameType.PathOfExile2, "One Hand Mace")]
    OneHandMace,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.onesword")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.onesword")]
    [ItemClassGameId(GameType.PathOfExile1, "One Hand Sword")]
    [ItemClassGameId(GameType.PathOfExile1, "Thrusting One Hand Sword")]
    [ItemClassGameId(GameType.PathOfExile2, "One Hand Sword")]
    OneHandSword,

    [ItemClassTradeId(GameType.PathOfExile2, "weapon.flail")]
    [ItemClassGameId(GameType.PathOfExile2, "Flail")]
    Flail,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.sceptre")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.sceptre")]
    [ItemClassGameId(GameType.PathOfExile1, "Sceptre")]
    [ItemClassGameId(GameType.PathOfExile2, "Sceptre")]
    Sceptre,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.staff")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.staff")]
    [ItemClassGameId(GameType.PathOfExile1, "Staff")]
    [ItemClassGameId(GameType.PathOfExile2, "Staff")]
    Staff,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.rod")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.rod")]
    [ItemClassGameId(GameType.PathOfExile1, "FishingRod")]
    [ItemClassGameId(GameType.PathOfExile2, "FishingRod")]
    FishingRod,

    [ItemClassTradeId(GameType.PathOfExile2, "weapon.talisman")]
    [ItemClassGameId(GameType.PathOfExile2, "Talisman")]
    Talisman,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.twoaxe")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.twoaxe")]
    [ItemClassGameId(GameType.PathOfExile1, "Two Hand Axe")]
    [ItemClassGameId(GameType.PathOfExile2, "Two Hand Axe")]
    TwoHandAxe,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.twomace")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.twomace")]
    [ItemClassGameId(GameType.PathOfExile1, "Two Hand Mace")]
    [ItemClassGameId(GameType.PathOfExile2, "Two Hand Mace")]
    TwoHandMace,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.twosword")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.twosword")]
    [ItemClassGameId(GameType.PathOfExile1, "Two Hand Sword")]
    [ItemClassGameId(GameType.PathOfExile2, "Two Hand Sword")]
    TwoHandSword,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.wand")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.wand")]
    [ItemClassGameId(GameType.PathOfExile1, "Wand")]
    [ItemClassGameId(GameType.PathOfExile2, "Wand")]
    Wand,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.warstaff")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.warstaff")]
    [ItemClassGameId(GameType.PathOfExile1, "Warstaff")]
    [ItemClassGameId(GameType.PathOfExile2, "Warstaff")]
    Warstaff,

    [ItemClassTradeId(GameType.PathOfExile2, "weapon.spear")]
    [ItemClassGameId(GameType.PathOfExile2, "Spear")]
    Spear,

    [ItemClassTradeId(GameType.PathOfExile1, "tincture")]
    [ItemClassGameId(GameType.PathOfExile1, "Tincture")]
    Tincture,

    [ItemClassTradeId(GameType.PathOfExile1, "sanctum.relic")]
    [ItemClassTradeId(GameType.PathOfExile2, "sanctum.relic")]
    [ItemClassGameId(GameType.PathOfExile1, "Relic")]
    [ItemClassGameId(GameType.PathOfExile1, "SanctumSpecialRelic")]
    [ItemClassGameId(GameType.PathOfExile1, "SmallRelic")]
    [ItemClassGameId(GameType.PathOfExile1, "MediumRelic")]
    [ItemClassGameId(GameType.PathOfExile1, "LargeRelic")]
    [ItemClassGameId(GameType.PathOfExile2, "Relic")]
    [ItemClassGameId(GameType.PathOfExile2, "SanctumSpecialRelic")]
    [ItemClassGameId(GameType.PathOfExile2, "SmallRelic")]
    [ItemClassGameId(GameType.PathOfExile2, "MediumRelic")]
    [ItemClassGameId(GameType.PathOfExile2, "LargeRelic")]
    SanctumRelic,

    [ItemClassTradeId(GameType.PathOfExile1, "idol")]
    [ItemClassGameId(GameType.PathOfExile1, "AtlasRelic")]
    Idol,

    [ItemClassTradeId(GameType.PathOfExile1, "gem.activegem")]
    [ItemClassTradeId(GameType.PathOfExile2, "gem.activegem")]
    [ItemClassGameId(GameType.PathOfExile1, "Active Skill Gem")]
    [ItemClassGameId(GameType.PathOfExile2, "Active Skill Gem")]
    ActiveSkillGem,

    [ItemClassTradeId(GameType.PathOfExile1, "gem.supportgem")]
    [ItemClassTradeId(GameType.PathOfExile2, "gem.supportgem")]
    [ItemClassGameId(GameType.PathOfExile1, "Support Skill Gem")]
    [ItemClassGameId(GameType.PathOfExile2, "Support Skill Gem")]
    SupportSkillGem,

    [ItemClassTradeId(GameType.PathOfExile1, "heistmission.contract")]
    [ItemClassGameId(GameType.PathOfExile1, "HeistContract")]
    HeistContract,

    [ItemClassTradeId(GameType.PathOfExile1, "heistmission.blueprint")]
    [ItemClassGameId(GameType.PathOfExile1, "HeistBlueprint")]
    HeistBlueprint,

    [ItemClassTradeId(GameType.PathOfExile1, "heistequipment.heistweapon")]
    [ItemClassGameId(GameType.PathOfExile1, "HeistEquipmentWeapon")]
    HeistEquipmentWeapon,

    [ItemClassTradeId(GameType.PathOfExile1, "heistequipment.heisttool")]
    [ItemClassGameId(GameType.PathOfExile1, "HeistEquipmentTool")]
    HeistEquipmentTool,

    [ItemClassTradeId(GameType.PathOfExile1, "heistequipment.heistutility")]
    [ItemClassGameId(GameType.PathOfExile1, "HeistEquipmentUtility")]
    HeistEquipmentUtility,

    [ItemClassTradeId(GameType.PathOfExile1, "heistequipment.heistreward")]
    [ItemClassGameId(GameType.PathOfExile1, "HeistEquipmentReward")]
    HeistEquipmentReward,

    [ItemClassTradeId(GameType.PathOfExile1, "currency.heistobjective")]
    [ItemClassGameId(GameType.PathOfExile1, "HeistObjective")]
    HeistObjective,

    [ItemClassTradeId(GameType.PathOfExile1, "logbook")]
    [ItemClassTradeId(GameType.PathOfExile2, "map.logbook")]
    [ItemClassGameId(GameType.PathOfExile1, "ExpeditionLogbook")]
    [ItemClassGameId(GameType.PathOfExile2, "ExpeditionLogbook")]
    ExpeditionLogbook,

    [ItemClassGameId(GameType.PathOfExile1, "ItemisedSanctum")]
    SanctumResearch,

    [ItemClassGameId(GameType.PathOfExile1, "MapFragment")]
    [ItemClassGameId(GameType.PathOfExile2, "MapFragment")]
    MapFragments,
}
