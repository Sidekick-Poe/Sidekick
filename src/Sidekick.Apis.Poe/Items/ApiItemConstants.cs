using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Items;

public class ApiItemConstants
{
    public static readonly Dictionary<string, (Category Category, bool UseRegex)> Poe1Categories = new()
    {
        { "accessory", (Category.Accessory, true) },
        { "armour", (Category.Armour, true) },
        { "card", (Category.DivinationCard, false) },
        { "currency", (Category.Currency, false) },
        { "flask", (Category.Flask, true) },
        { "gem", (Category.Gem, false) },
        { "jewel", (Category.Jewel, true) },
        { "map", (Category.Map, true) },
        { "weapon", (Category.Weapon, true) },
        { "leaguestone", (Category.Leaguestone, false) },
        { "monster", (Category.ItemisedMonster, true) },
        { "heistequipment", (Category.HeistEquipment, true) },
        { "heistmission", (Category.Contract, true) },
        { "logbook", (Category.Logbook, true) },
        { "sanctum", (Category.Sanctum, true) },
        { "memoryline", (Category.MemoryLine, true) },
        { "tincture", (Category.Tincture, true) },
        { "corpse", (Category.Corpse, true) },
        { "idol", (Category.Idol, true) },
    };

    public static readonly Dictionary<string, (Category Category, bool UseRegex)> Poe2Categories = new()
    {
        { "accessory", (Category.Accessory, true) },
        { "armour", (Category.Armour, true) },
        { "currency", (Category.Currency, false) },
        { "flask", (Category.Flask, true) },
        { "gem", (Category.Gem, false) },
        { "jewel", (Category.Jewel, true) },
        { "map", (Category.Map, true) },
        { "weapon", (Category.Weapon, true) },
        { "sanctum", (Category.Sanctum, true) },
    };
}
