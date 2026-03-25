namespace Sidekick.Data.Items;

public enum ItemClass
{
    Unknown = 0,

    [ItemClassGameId(GameType.PathOfExile1, "RemovedItem")]
    RemovedItem,

    [ItemClassTradeId(GameType.PathOfExile1, "accessory.amulet")]
    [ItemClassTradeId(GameType.PathOfExile2, "accessory.amulet")]
    [ItemClassGameId(GameType.PathOfExile1, "Amulet")]
    Amulet,

    [ItemClassTradeId(GameType.PathOfExile1, "accessory.belt")]
    [ItemClassTradeId(GameType.PathOfExile2, "accessory.belt")]
    [ItemClassGameId(GameType.PathOfExile1, "Belt")]
    Belt,

    [ItemClassTradeId(GameType.PathOfExile1, "accessory.ring")]
    [ItemClassTradeId(GameType.PathOfExile2, "accessory.ring")]
    [ItemClassGameId(GameType.PathOfExile1, "Ring")]
    Ring,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.chest")]
    [ItemClassTradeId(GameType.PathOfExile2, "armour.chest")]
    [ItemClassGameId(GameType.PathOfExile1, "Body Armour")]
    BodyArmour,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.boots")]
    [ItemClassTradeId(GameType.PathOfExile2, "armour.boots")]
    [ItemClassGameId(GameType.PathOfExile1, "Boots")]
    Boots,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.gloves")]
    [ItemClassTradeId(GameType.PathOfExile2, "armour.gloves")]
    [ItemClassGameId(GameType.PathOfExile1, "Gloves")]
    Gloves,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.helmet")]
    [ItemClassTradeId(GameType.PathOfExile2, "armour.helmet")]
    [ItemClassGameId(GameType.PathOfExile1, "Helmet")]
    Helmet,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.quiver")]
    [ItemClassTradeId(GameType.PathOfExile2, "armour.quiver")]
    [ItemClassGameId(GameType.PathOfExile1, "Quiver")]
    Quiver,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.shield")]
    [ItemClassTradeId(GameType.PathOfExile2, "armour.shield")]
    [ItemClassGameId(GameType.PathOfExile1, "Shield")]
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

    [ItemClassTradeId(GameType.PathOfExile1, "currency.heistobjective")]
    HeistObjective,

    [ItemClassTradeId(GameType.PathOfExile1, "flask")]
    [ItemClassTradeId(GameType.PathOfExile2, "flask")]
    [ItemClassGameId(GameType.PathOfExile1, "Flask")]
    [ItemClassGameId(GameType.PathOfExile1, "UtilityFlask")]
    [ItemClassGameId(GameType.PathOfExile1, "HybridFlask")]
    [ItemClassGameId(GameType.PathOfExile1, "LifeFlask")]
    [ItemClassGameId(GameType.PathOfExile1, "ManaFlask")]
    Flask,

    [ItemClassTradeId(GameType.PathOfExile2, "flask.life")]
    LifeFlask,

    [ItemClassTradeId(GameType.PathOfExile2, "flask.mana")]
    ManaFlask,

    [ItemClassGameId(GameType.PathOfExile1, "AnimalCharm")]
    Charms,

    [ItemClassTradeId(GameType.PathOfExile1, "heistmission.blueprint")]
    [ItemClassGameId(GameType.PathOfExile1, "HeistBlueprint")]
    Blueprint,

    [ItemClassTradeId(GameType.PathOfExile1, "heistmission.contract")]
    [ItemClassGameId(GameType.PathOfExile1, "HeistContract")]
    Contract,

    [ItemClassTradeId(GameType.PathOfExile1, "heistequipment.heistreward")]
    [ItemClassGameId(GameType.PathOfExile1, "HeistEquipmentReward")]
    HeistReward,

    [ItemClassTradeId(GameType.PathOfExile1, "heistequipment")]
    HeistEquipment,

    [ItemClassTradeId(GameType.PathOfExile1, "heistmission")]
    HeistMission,

    [ItemClassTradeId(GameType.PathOfExile1, "heistequipment.heistutility")]
    [ItemClassGameId(GameType.PathOfExile1, "HeistEquipmentUtility")]
    HeistUtility,

    [ItemClassTradeId(GameType.PathOfExile1, "heistequipment.heistweapon")]
    [ItemClassGameId(GameType.PathOfExile1, "HeistEquipmentWeapon")]
    HeistWeapon,

    [ItemClassTradeId(GameType.PathOfExile1, "heistequipment.heisttool")]
    [ItemClassGameId(GameType.PathOfExile1, "HeistEquipmentTool")]
    HeistTool,

    [ItemClassTradeId(GameType.PathOfExile1, "jewel")]
    [ItemClassTradeId(GameType.PathOfExile2, "jewel")]
    [ItemClassGameId(GameType.PathOfExile1, "Jewel")]
    Jewel,

    [ItemClassTradeId(GameType.PathOfExile1, "jewel.abyss")]
    [ItemClassGameId(GameType.PathOfExile1, "AbyssJewel")]
    AbyssJewel,

    [ItemClassTradeId(GameType.PathOfExile1, "logbook")]
    [ItemClassTradeId(GameType.PathOfExile2, "map.logbook")]
    [ItemClassGameId(GameType.PathOfExile1, "ExpeditionLogbook")]
    Logbook,

    [ItemClassTradeId(GameType.PathOfExile2, "map.waystone")]
    [ItemClassGameId(GameType.PathOfExile2, "MapKey")]
    Waystone,

    [ItemClassTradeId(GameType.PathOfExile2, "map.barya")]
    Barya,

    [ItemClassTradeId(GameType.PathOfExile2, "map.bosskey")]
    BossKey,

    [ItemClassTradeId(GameType.PathOfExile2, "map.ultimatum")]
    Ultimatum,

    [ItemClassTradeId(GameType.PathOfExile2, "map.tablet")]
    Tablet,

    [ItemClassTradeId(GameType.PathOfExile1, "map")]
    [ItemClassTradeId(GameType.PathOfExile2, "map")]
    [ItemClassGameId(GameType.PathOfExile1, "Map")]
    [ItemClassGameId(GameType.PathOfExile1, "MapKey")]
    [ItemClassGameId(GameType.PathOfExile1, "InstanceLocalItem")]
    Map,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.bow")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.bow")]
    [ItemClassGameId(GameType.PathOfExile1, "Bow")]
    Bow,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.crossbow")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.crossbow")]
    Crossbow,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.claw")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.claw")]
    [ItemClassGameId(GameType.PathOfExile1, "Claw")]
    Claw,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.dagger")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.dagger")]
    [ItemClassGameId(GameType.PathOfExile1, "Dagger")]
    [ItemClassGameId(GameType.PathOfExile1, "Rune Dagger")]
    Dagger,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.oneaxe")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.oneaxe")]
    [ItemClassGameId(GameType.PathOfExile1, "One Hand Axe")]
    OneHandAxe,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.onemace")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.onemace")]
    [ItemClassGameId(GameType.PathOfExile1, "One Hand Mace")]
    OneHandMace,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.onesword")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.onesword")]
    [ItemClassGameId(GameType.PathOfExile1, "One Hand Sword")]
    [ItemClassGameId(GameType.PathOfExile1, "Thrusting One Hand Sword")]
    OneHandSword,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.sceptre")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.sceptre")]
    [ItemClassGameId(GameType.PathOfExile1, "Sceptre")]
    Sceptre,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.staff")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.staff")]
    [ItemClassGameId(GameType.PathOfExile1, "Staff")]
    Staff,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.rod")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.rod")]
    [ItemClassGameId(GameType.PathOfExile1, "FishingRod")]
    FishingRod,

    [ItemClassTradeId(GameType.PathOfExile2, "weapon.talisman")]
    Talisman,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.twoaxe")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.twoaxe")]
    [ItemClassGameId(GameType.PathOfExile1, "Two Hand Axe")]
    TwoHandAxe,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.twomace")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.twomace")]
    [ItemClassGameId(GameType.PathOfExile1, "Two Hand Mace")]
    TwoHandMace,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.twosword")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.twosword")]
    [ItemClassGameId(GameType.PathOfExile1, "Two Hand Sword")]
    TwoHandSword,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.wand")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.wand")]
    [ItemClassGameId(GameType.PathOfExile1, "Wand")]
    Wand,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.warstaff")]
    [ItemClassTradeId(GameType.PathOfExile2, "weapon.warstaff")]
    [ItemClassGameId(GameType.PathOfExile1, "Warstaff")]
    Warstaff,

    [ItemClassTradeId(GameType.PathOfExile2, "weapon.spear")]
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
    SanctumRelic,

    [ItemClassTradeId(GameType.PathOfExile1, "idol")]
    [ItemClassTradeId(GameType.PathOfExile2, "currency.idol")]
    [ItemClassGameId(GameType.PathOfExile1, "AtlasRelic")]
    Idol,

    [ItemClassTradeId(GameType.PathOfExile1, "gem.activegem")]
    [ItemClassTradeId(GameType.PathOfExile2, "gem.activegem")]
    [ItemClassGameId(GameType.PathOfExile1, "Active Skill Gem")]
    ActiveSkillGem,
}
