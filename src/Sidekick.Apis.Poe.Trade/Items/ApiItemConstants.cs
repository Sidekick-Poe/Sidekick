using Sidekick.Apis.Poe.Items;
namespace Sidekick.Apis.Poe.Trade.Items;

public class ApiItemConstants
{
    public static readonly Dictionary<string, Category> Poe1Categories = new()
    {
        { "accessory", Category.Accessory },
        { "armour", Category.Armour },
        { "card", Category.DivinationCard },
        { "currency", Category.Currency },
        { "flask", Category.Flask },
        { "gem", Category.Gem },
        { "jewel", Category.Jewel },
        { "map", Category.Map },
        { "weapon", Category.Weapon },
        { "leaguestone", Category.Leaguestone },
        { "monster", Category.ItemisedMonster },
        { "heistequipment", Category.HeistEquipment },
        { "heistmission", Category.Contract },
        { "logbook", Category.Logbook },
        { "sanctum", Category.Sanctum },
        { "tincture", Category.Tincture },
        { "corpse", Category.Corpse },
        { "idol", Category.Idol },
    };

    public static readonly Dictionary<string, Category> Poe2Categories = new()
    {
        { "accessory", Category.Accessory },
        { "armour", Category.Armour },
        { "currency", Category.Currency },
        { "flask", Category.Flask },
        { "gem", Category.Gem },
        { "jewel", Category.Jewel },
        { "map", Category.Map },
        { "weapon", Category.Weapon },
        { "sanctum", Category.Sanctum },
    };
}
