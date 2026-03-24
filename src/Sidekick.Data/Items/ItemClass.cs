namespace Sidekick.Data.Items;

public enum ItemClass
{
    Unknown = 0,

    [ItemClassGameId(GameType.PathOfExile1, "RemovedItem")]
    RemovedItem,

    [ItemClassTradeId(GameType.PathOfExile1, "accessory.amulet")]
    [ItemClassGameId(GameType.PathOfExile1, "Amulet")]
    Amulet,

    [ItemClassTradeId(GameType.PathOfExile1, "accessory.belt")]
    [ItemClassGameId(GameType.PathOfExile1, "Belt")]
    Belt,

    [ItemClassTradeId(GameType.PathOfExile1, "accessory.ring")]
    [ItemClassGameId(GameType.PathOfExile1, "Ring")]
    Ring,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.chest")]
    [ItemClassGameId(GameType.PathOfExile1, "Body Armour")]
    BodyArmour,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.boots")]
    [ItemClassGameId(GameType.PathOfExile1, "Boots")]
    Boots,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.gloves")]
    [ItemClassGameId(GameType.PathOfExile1, "Gloves")]
    Gloves,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.helmet")]
    [ItemClassGameId(GameType.PathOfExile1, "Helmet")]
    Helmet,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.quiver")]
    [ItemClassGameId(GameType.PathOfExile1, "Quiver")]
    Quiver,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.shield")]
    [ItemClassGameId(GameType.PathOfExile1, "Shield")]
    Shield,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.focus")]
    [ItemClassGameId(GameType.PathOfExile2, "Focus")]
    Focus,

    [ItemClassTradeId(GameType.PathOfExile1, "armour.buckler")]
    [ItemClassGameId(GameType.PathOfExile2, "Buckler")]
    Buckler,

    [ItemClassTradeId(GameType.PathOfExile1, "card")]
    [ItemClassGameId(GameType.PathOfExile1, "DivinationCard")]
    DivinationCard,

    [ItemClassTradeId(GameType.PathOfExile1, "accessory.trinket")]
    [ItemClassGameId(GameType.PathOfExile1, "Trinket")]
    Trinket,

    [ItemClassTradeId(GameType.PathOfExile1, "currency.heistobjective")]
    HeistObjective,

    [ItemClassTradeId(GameType.PathOfExile1, "flask")]
    [ItemClassGameId(GameType.PathOfExile1, "Flask")]
    [ItemClassGameId(GameType.PathOfExile1, "UtilityFlask")]
    Flask,

    [ItemClassTradeId(GameType.PathOfExile1, "flask.life")]
    [ItemClassGameId(GameType.PathOfExile1, "LifeFlask")]
    LifeFlask,

    [ItemClassTradeId(GameType.PathOfExile1, "flask.mana")]
    [ItemClassGameId(GameType.PathOfExile1, "ManaFlask")]
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
    [ItemClassGameId(GameType.PathOfExile1, "Jewel")]
    Jewel,

    [ItemClassTradeId(GameType.PathOfExile1, "jewel.abyss")]
    [ItemClassGameId(GameType.PathOfExile1, "AbyssJewel")]
    AbyssJewel,

    [ItemClassTradeId(GameType.PathOfExile1, "logbook")]
    [ItemClassGameId(GameType.PathOfExile1, "ExpeditionLogbook")]
    Logbook,

    [ItemClassTradeId(GameType.PathOfExile2, "map.waystone")]
    [ItemClassGameId(GameType.PathOfExile2, "MapKey")]
    Waystone,

    [ItemClassTradeId(GameType.PathOfExile1, "map.breachstone")]
    [ItemClassGameId(GameType.PathOfExile1, "Breachstone")]
    Breachstone,

    [ItemClassTradeId(GameType.PathOfExile1, "map.barya")]
    Barya,

    [ItemClassTradeId(GameType.PathOfExile1, "map.bosskey")]
    BossKey,

    [ItemClassTradeId(GameType.PathOfExile1, "map.ultimatum")]
    Ultimatum,

    [ItemClassTradeId(GameType.PathOfExile1, "map.tablet")]
    Tablet,

    [ItemClassTradeId(GameType.PathOfExile1, "map")]
    [ItemClassGameId(GameType.PathOfExile1, "Map")]
    [ItemClassGameId(GameType.PathOfExile1, "MapKey")]
    [ItemClassGameId(GameType.PathOfExile1, "InstanceLocalItem")]
    Map,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.bow")]
    [ItemClassGameId(GameType.PathOfExile1, "Bow")]
    Bow,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.crossbow")]
    Crossbow,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.claw")]
    [ItemClassGameId(GameType.PathOfExile1, "Claw")]
    Claw,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.dagger")]
    [ItemClassGameId(GameType.PathOfExile1, "Dagger")]
    [ItemClassGameId(GameType.PathOfExile1, "Rune Dagger")]
    Dagger,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.oneaxe")]
    [ItemClassGameId(GameType.PathOfExile1, "One Hand Axe")]
    OneHandAxe,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.onemace")]
    [ItemClassTradeId(GameType.PathOfExile1, "weapon.basemace")]
    [ItemClassGameId(GameType.PathOfExile1, "One Hand Mace")]
    OneHandMace,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.onesword")]
    [ItemClassGameId(GameType.PathOfExile1, "One Hand Sword")]
    [ItemClassGameId(GameType.PathOfExile1, "Thrusting One Hand Sword")]
    OneHandSword,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.sceptre")]
    [ItemClassGameId(GameType.PathOfExile1, "Sceptre")]
    Sceptre,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.staff")]
    [ItemClassGameId(GameType.PathOfExile1, "Staff")]
    Staff,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.rod")]
    [ItemClassGameId(GameType.PathOfExile1, "FishingRod")]
    FishingRod,

    [ItemClassTradeId(GameType.PathOfExile2, "weapon.talisman")]
    Talisman,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.twoaxe")]
    [ItemClassGameId(GameType.PathOfExile1, "Two Hand Axe")]
    TwoHandAxe,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.twomace")]
    [ItemClassGameId(GameType.PathOfExile1, "Two Hand Mace")]
    TwoHandMace,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.twosword")]
    [ItemClassGameId(GameType.PathOfExile1, "Two Hand Sword")]
    TwoHandSword,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.wand")]
    [ItemClassGameId(GameType.PathOfExile1, "Wand")]
    Wand,

    [ItemClassTradeId(GameType.PathOfExile1, "weapon.warstaff")]
    [ItemClassGameId(GameType.PathOfExile1, "Warstaff")]
    Warstaff,

    [ItemClassTradeId(GameType.PathOfExile2, "weapon.spear")]
    Spear,

    [ItemClassTradeId(GameType.PathOfExile1, "tincture")]
    [ItemClassGameId(GameType.PathOfExile1, "Tincture")]
    Tincture,

    [ItemClassTradeId(GameType.PathOfExile1, "sanctum.relic")]
    [ItemClassGameId(GameType.PathOfExile1, "Relic")]
    [ItemClassGameId(GameType.PathOfExile1, "SanctumSpecialRelic")]
    [ItemClassGameId(GameType.PathOfExile1, "SmallRelic")]
    [ItemClassGameId(GameType.PathOfExile1, "MediumRelic")]
    [ItemClassGameId(GameType.PathOfExile1, "LargeRelic")]
    SanctumRelic,

    [ItemClassTradeId(GameType.PathOfExile1, "idol")]
    [ItemClassGameId(GameType.PathOfExile1, "AtlasRelic")]
    Idol,
}
