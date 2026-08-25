namespace Sidekick.Game.ItemClasses;

public enum ItemClass
{
    Unknown = 0,

    [ItemClassTradeId(GameType.Poe1, "accessory.amulet")]
    [ItemClassTradeId(GameType.Poe2, "accessory.amulet")]
    [ItemClassGameId(GameType.Poe1, "Amulet")]
    [ItemClassGameId(GameType.Poe2, "Amulet")]
    Amulet,

    [ItemClassTradeId(GameType.Poe1, "accessory.belt")]
    [ItemClassTradeId(GameType.Poe2, "accessory.belt")]
    [ItemClassGameId(GameType.Poe1, "Belt")]
    [ItemClassGameId(GameType.Poe2, "Belt")]
    Belt,

    [ItemClassTradeId(GameType.Poe1, "accessory.ring")]
    [ItemClassTradeId(GameType.Poe2, "accessory.ring")]
    [ItemClassGameId(GameType.Poe1, "Ring")]
    [ItemClassGameId(GameType.Poe2, "Ring")]
    Ring,

    [ItemClassTradeId(GameType.Poe1, "armour.chest")]
    [ItemClassTradeId(GameType.Poe2, "armour.chest")]
    [ItemClassGameId(GameType.Poe1, "Body Armour")]
    [ItemClassGameId(GameType.Poe2, "Body Armour")]
    BodyArmour,

    [ItemClassTradeId(GameType.Poe1, "armour.boots")]
    [ItemClassTradeId(GameType.Poe2, "armour.boots")]
    [ItemClassGameId(GameType.Poe1, "Boots")]
    [ItemClassGameId(GameType.Poe2, "Boots")]
    Boots,

    [ItemClassTradeId(GameType.Poe1, "armour.gloves")]
    [ItemClassTradeId(GameType.Poe2, "armour.gloves")]
    [ItemClassGameId(GameType.Poe1, "Gloves")]
    [ItemClassGameId(GameType.Poe2, "Gloves")]
    Gloves,

    [ItemClassTradeId(GameType.Poe1, "armour.helmet")]
    [ItemClassTradeId(GameType.Poe2, "armour.helmet")]
    [ItemClassGameId(GameType.Poe1, "Helmet")]
    [ItemClassGameId(GameType.Poe2, "Helmet")]
    Helmet,

    [ItemClassTradeId(GameType.Poe1, "armour.quiver")]
    [ItemClassTradeId(GameType.Poe2, "armour.quiver")]
    [ItemClassGameId(GameType.Poe1, "Quiver")]
    [ItemClassGameId(GameType.Poe2, "Quiver")]
    Quiver,

    [ItemClassTradeId(GameType.Poe1, "armour.shield")]
    [ItemClassTradeId(GameType.Poe2, "armour.shield")]
    [ItemClassGameId(GameType.Poe1, "Shield")]
    [ItemClassGameId(GameType.Poe2, "Shield")]
    Shield,

    [ItemClassTradeId(GameType.Poe2, "armour.focus")]
    [ItemClassGameId(GameType.Poe2, "Focus")]
    Focus,

    [ItemClassTradeId(GameType.Poe2, "armour.buckler")]
    [ItemClassGameId(GameType.Poe2, "Buckler")]
    Buckler,

    [ItemClassTradeId(GameType.Poe1, "accessory.trinket")]
    [ItemClassGameId(GameType.Poe1, "Trinket")]
    Trinket,

    [ItemClassTradeId(GameType.Poe1, "flask")]
    [ItemClassTradeId(GameType.Poe2, "flask")]
    [ItemClassGameId(GameType.Poe1, "Flask")]
    [ItemClassGameId(GameType.Poe1, "UtilityFlask")]
    [ItemClassGameId(GameType.Poe1, "HybridFlask")]
    [ItemClassGameId(GameType.Poe1, "LifeFlask")]
    [ItemClassGameId(GameType.Poe1, "ManaFlask")]
    Flask,

    [ItemClassTradeId(GameType.Poe2, "flask.life")]
    [ItemClassGameId(GameType.Poe2, "LifeFlask")]
    LifeFlask,

    [ItemClassTradeId(GameType.Poe2, "flask.mana")]
    [ItemClassGameId(GameType.Poe2, "ManaFlask")]
    ManaFlask,

    [ItemClassGameId(GameType.Poe1, "AnimalCharm")]
    [ItemClassGameId(GameType.Poe2, "UtilityFlask")]
    Charms,

    [ItemClassTradeId(GameType.Poe1, "jewel")]
    [ItemClassTradeId(GameType.Poe2, "jewel")]
    [ItemClassGameId(GameType.Poe1, "Jewel")]
    [ItemClassGameId(GameType.Poe2, "Jewel")]
    Jewel,

    [ItemClassTradeId(GameType.Poe1, "jewel.abyss")]
    [ItemClassGameId(GameType.Poe1, "AbyssJewel")]
    AbyssJewel,

    [ItemClassTradeId(GameType.Poe2, "map.barya")]
    Barya,

    [ItemClassTradeId(GameType.Poe2, "map.bosskey")]
    BossKey,

    [ItemClassTradeId(GameType.Poe2, "map.ultimatum")]
    Ultimatum,

    [ItemClassTradeId(GameType.Poe2, "map.tablet")]
    [ItemClassGameId(GameType.Poe2, "TowerAugmentation")]
    Tablet,

    [ItemClassTradeId(GameType.Poe1, "map")]
    [ItemClassTradeId(GameType.Poe2, "map.waystone")]
    [ItemClassGameId(GameType.Poe1, "Map")]
    [ItemClassGameId(GameType.Poe1, "MapKey")]
    [ItemClassGameId(GameType.Poe1, "InstanceLocalItem")]
    [ItemClassGameId(GameType.Poe2, "Map")]
    [ItemClassGameId(GameType.Poe2, "MapKey")]
    Map,

    [ItemClassTradeId(GameType.Poe1, "weapon.bow")]
    [ItemClassTradeId(GameType.Poe2, "weapon.bow")]
    [ItemClassGameId(GameType.Poe1, "Bow")]
    [ItemClassGameId(GameType.Poe2, "Bow")]
    Bow,

    [ItemClassTradeId(GameType.Poe1, "weapon.crossbow")]
    [ItemClassTradeId(GameType.Poe2, "weapon.crossbow")]
    [ItemClassGameId(GameType.Poe2, "Crossbow")]
    Crossbow,

    [ItemClassTradeId(GameType.Poe1, "weapon.claw")]
    [ItemClassTradeId(GameType.Poe2, "weapon.claw")]
    [ItemClassGameId(GameType.Poe1, "Claw")]
    [ItemClassGameId(GameType.Poe2, "Claw")]
    Claw,

    [ItemClassTradeId(GameType.Poe1, "weapon.dagger")]
    [ItemClassTradeId(GameType.Poe2, "weapon.dagger")]
    [ItemClassGameId(GameType.Poe1, "Dagger")]
    [ItemClassGameId(GameType.Poe1, "Rune Dagger")]
    [ItemClassGameId(GameType.Poe2, "Dagger")]
    Dagger,

    [ItemClassTradeId(GameType.Poe1, "weapon.oneaxe")]
    [ItemClassTradeId(GameType.Poe2, "weapon.oneaxe")]
    [ItemClassGameId(GameType.Poe1, "One Hand Axe")]
    [ItemClassGameId(GameType.Poe2, "One Hand Axe")]
    OneHandAxe,

    [ItemClassTradeId(GameType.Poe1, "weapon.onemace")]
    [ItemClassTradeId(GameType.Poe2, "weapon.onemace")]
    [ItemClassGameId(GameType.Poe1, "One Hand Mace")]
    [ItemClassGameId(GameType.Poe2, "One Hand Mace")]
    OneHandMace,

    [ItemClassTradeId(GameType.Poe1, "weapon.onesword")]
    [ItemClassTradeId(GameType.Poe2, "weapon.onesword")]
    [ItemClassGameId(GameType.Poe1, "One Hand Sword")]
    [ItemClassGameId(GameType.Poe1, "Thrusting One Hand Sword")]
    [ItemClassGameId(GameType.Poe2, "One Hand Sword")]
    OneHandSword,

    [ItemClassTradeId(GameType.Poe2, "weapon.flail")]
    [ItemClassGameId(GameType.Poe2, "Flail")]
    Flail,

    [ItemClassTradeId(GameType.Poe1, "weapon.sceptre")]
    [ItemClassTradeId(GameType.Poe2, "weapon.sceptre")]
    [ItemClassGameId(GameType.Poe1, "Sceptre")]
    [ItemClassGameId(GameType.Poe2, "Sceptre")]
    Sceptre,

    [ItemClassTradeId(GameType.Poe1, "weapon.staff")]
    [ItemClassTradeId(GameType.Poe2, "weapon.staff")]
    [ItemClassGameId(GameType.Poe1, "Staff")]
    [ItemClassGameId(GameType.Poe2, "Staff")]
    Staff,

    [ItemClassTradeId(GameType.Poe1, "weapon.rod")]
    [ItemClassTradeId(GameType.Poe2, "weapon.rod")]
    [ItemClassGameId(GameType.Poe1, "FishingRod")]
    [ItemClassGameId(GameType.Poe2, "FishingRod")]
    FishingRod,

    [ItemClassTradeId(GameType.Poe2, "weapon.talisman")]
    [ItemClassGameId(GameType.Poe2, "Talisman")]
    Talisman,

    [ItemClassTradeId(GameType.Poe1, "weapon.twoaxe")]
    [ItemClassTradeId(GameType.Poe2, "weapon.twoaxe")]
    [ItemClassGameId(GameType.Poe1, "Two Hand Axe")]
    [ItemClassGameId(GameType.Poe2, "Two Hand Axe")]
    TwoHandAxe,

    [ItemClassTradeId(GameType.Poe1, "weapon.twomace")]
    [ItemClassTradeId(GameType.Poe2, "weapon.twomace")]
    [ItemClassGameId(GameType.Poe1, "Two Hand Mace")]
    [ItemClassGameId(GameType.Poe2, "Two Hand Mace")]
    TwoHandMace,

    [ItemClassTradeId(GameType.Poe1, "weapon.twosword")]
    [ItemClassTradeId(GameType.Poe2, "weapon.twosword")]
    [ItemClassGameId(GameType.Poe1, "Two Hand Sword")]
    [ItemClassGameId(GameType.Poe2, "Two Hand Sword")]
    TwoHandSword,

    [ItemClassTradeId(GameType.Poe1, "weapon.wand")]
    [ItemClassTradeId(GameType.Poe2, "weapon.wand")]
    [ItemClassGameId(GameType.Poe1, "Wand")]
    [ItemClassGameId(GameType.Poe2, "Wand")]
    Wand,

    [ItemClassTradeId(GameType.Poe1, "weapon.warstaff")]
    [ItemClassTradeId(GameType.Poe2, "weapon.warstaff")]
    [ItemClassGameId(GameType.Poe1, "Warstaff")]
    [ItemClassGameId(GameType.Poe2, "Warstaff")]
    Warstaff,

    [ItemClassTradeId(GameType.Poe2, "weapon.spear")]
    [ItemClassGameId(GameType.Poe2, "Spear")]
    Spear,

    [ItemClassTradeId(GameType.Poe1, "tincture")]
    [ItemClassGameId(GameType.Poe1, "Tincture")]
    Tincture,

    [ItemClassTradeId(GameType.Poe1, "sanctum.relic")]
    [ItemClassTradeId(GameType.Poe2, "sanctum.relic")]
    [ItemClassGameId(GameType.Poe1, "Relic")]
    [ItemClassGameId(GameType.Poe1, "SanctumSpecialRelic")]
    [ItemClassGameId(GameType.Poe1, "SmallRelic")]
    [ItemClassGameId(GameType.Poe1, "MediumRelic")]
    [ItemClassGameId(GameType.Poe1, "LargeRelic")]
    [ItemClassGameId(GameType.Poe2, "Relic")]
    [ItemClassGameId(GameType.Poe2, "SanctumSpecialRelic")]
    [ItemClassGameId(GameType.Poe2, "SmallRelic")]
    [ItemClassGameId(GameType.Poe2, "MediumRelic")]
    [ItemClassGameId(GameType.Poe2, "LargeRelic")]
    SanctumRelic,

    [ItemClassTradeId(GameType.Poe1, "idol")]
    [ItemClassGameId(GameType.Poe1, "AtlasRelic")]
    Idol,

    [ItemClassTradeId(GameType.Poe1, "gem.activegem")]
    [ItemClassTradeId(GameType.Poe2, "gem.activegem")]
    [ItemClassGameId(GameType.Poe1, "Active Skill Gem")]
    [ItemClassGameId(GameType.Poe2, "Active Skill Gem")]
    ActiveSkillGem,

    [ItemClassTradeId(GameType.Poe1, "gem.supportgem")]
    [ItemClassTradeId(GameType.Poe2, "gem.supportgem")]
    [ItemClassGameId(GameType.Poe1, "Support Skill Gem")]
    [ItemClassGameId(GameType.Poe2, "Support Skill Gem")]
    SupportSkillGem,

    [ItemClassTradeId(GameType.Poe1, "heistmission.contract")]
    [ItemClassGameId(GameType.Poe1, "HeistContract")]
    HeistContract,

    [ItemClassTradeId(GameType.Poe1, "heistmission.blueprint")]
    [ItemClassGameId(GameType.Poe1, "HeistBlueprint")]
    HeistBlueprint,

    [ItemClassTradeId(GameType.Poe1, "heistequipment.heistweapon")]
    [ItemClassGameId(GameType.Poe1, "HeistEquipmentWeapon")]
    HeistEquipmentWeapon,

    [ItemClassTradeId(GameType.Poe1, "heistequipment.heisttool")]
    [ItemClassGameId(GameType.Poe1, "HeistEquipmentTool")]
    HeistEquipmentTool,

    [ItemClassTradeId(GameType.Poe1, "heistequipment.heistutility")]
    [ItemClassGameId(GameType.Poe1, "HeistEquipmentUtility")]
    HeistEquipmentUtility,

    [ItemClassTradeId(GameType.Poe1, "heistequipment.heistreward")]
    [ItemClassGameId(GameType.Poe1, "HeistEquipmentReward")]
    HeistEquipmentReward,

    [ItemClassTradeId(GameType.Poe1, "currency.heistobjective")]
    [ItemClassGameId(GameType.Poe1, "HeistObjective")]
    HeistObjective,

    [ItemClassTradeId(GameType.Poe1, "logbook")]
    [ItemClassTradeId(GameType.Poe2, "map.logbook")]
    [ItemClassGameId(GameType.Poe1, "ExpeditionLogbook")]
    [ItemClassGameId(GameType.Poe2, "ExpeditionLogbook")]
    ExpeditionLogbook,

    [ItemClassGameId(GameType.Poe1, "ItemisedSanctum")]
    SanctumResearch,

    [ItemClassGameId(GameType.Poe1, "MapFragment")]
    [ItemClassGameId(GameType.Poe2, "MapFragment")]
    MapFragments,

    [ItemClassTradeId(GameType.Poe1, "chart")]
    [ItemClassGameId(GameType.Poe1, "DeepwaterChart")]
    Chart,
}
