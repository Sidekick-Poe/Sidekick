using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Items;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class HeistParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void HeistTool()
    {
        var actual = parser.ParseItem(@"Item Class: Heist Tools
Rarity: Magic
Skillful Basic Disguise Kit
--------
This item can be equipped by:
Niles, the Interrogator
Gianna, the Master of Disguise
--------
Requirements:
Level 2 in Deception
--------
Item Level: 69
--------
6% increased Deception speed (implicit)
--------
12% increased Job speed
--------
Can only be equipped to Heist members.
");

        Assert.Equal(ItemClass.HeistEquipmentTool, actual.Definition.ItemClass.Type);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Basic Disguise Kit", actual.Definition.TradeItem?.Type);
    }

    [Fact]
    public void HeistCloak()
    {
        var actual = parser.ParseItem(@"Item Class: Heist Cloaks
Rarity: Normal
Torn Cloak
--------
Any Heist member can equip this item.
--------
Requirements:
Level 2 in Any Job
--------
Item Level: 67
--------
2% reduced raising of Alert Level (implicit)
--------
Can only be equipped to Heist members.
");

        Assert.Equal(ItemClass.HeistEquipmentUtility, actual.Definition.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Torn Cloak", actual.Definition.TradeItem?.Type);
    }

    [Fact]
    public void HeistBrooch()
    {
        var actual = parser.ParseItem(@"Item Class: Heist Brooches
Rarity: Normal
Silver Brooch
--------
Any Heist member can equip this item.
--------
Requirements:
Level 2 in Any Job
--------
Item Level: 73
--------
12% increased Rarity of Items dropped in Heists (implicit)
--------
Can only be equipped to Heist members.
");

        Assert.Equal(ItemClass.HeistEquipmentReward, actual.Definition.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Silver Brooch", actual.Definition.TradeItem?.Type);
    }

    [Fact]
    public void HeistGear()
    {
        var actual = parser.ParseItem(@"Item Class: Heist Gear
Rarity: Rare
Miracle Equipment
Rough Sharpening Stone
--------
Any Heist member can equip this item.
--------
Requirements:
Level 2 in Any Job
--------
Item Level: 69
--------
13% increased Melee Damage (implicit)
--------
27% increased Melee Damage
20 to 25 added Fire Damage
Players and their Minions have 20 to 25 added Fire Damage
68% increased Critical Strike Chance
Grants Level 10 Anger Skill
--------
Can only be equipped to Heist members.
");

        Assert.Equal(ItemClass.HeistEquipmentWeapon, actual.Definition.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Rough Sharpening Stone", actual.Definition.TradeItem?.Type);
    }

    [Fact]
    public void HeistTarget()
    {
        var actual = parser.ParseItem(@"Item Class: Heist Targets
Rarity: Currency
Golden Napuatzi Idol
--------
""I believe she was a beautiful maiden.
Kind, virtuous and devout through - and - through.
Such a gift will set me apart from all other suitors.""
--------
Can be exchanged with Faustus, the Fence in The Rogue Harbour
");

        Assert.Equal(ItemClass.Unknown, actual.Definition.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Golden Napuatzi Idol", actual.Definition.TradeItem?.Type);
    }

    [Fact]
    public void ThiefTrinket()
    {
        var actual = parser.ParseItem(@"Item Class: Trinkets
Rarity: Rare
Armageddon Lure
Thief's Trinket
--------
Requirements:
Level: 48
--------
Item Level: 67
--------
2% chance to receive additional Breach items when opening a Reward Chest in a Heist
1% chance to receive additional Delirium items when opening a Reward Chest in a Heist
4% increased Quantity of Items dropped in Heists
--------
You must find the sculpture The Catch in a Smuggler's Den or Underbelly Blueprint to be able to equip this
--------
Corrupted
");

        Assert.Equal(ItemClass.Unknown, actual.Definition.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Thief's Trinket", actual.Definition.TradeItem?.Type);
        Assert.True(actual.Properties.Corrupted);
    }

    [Fact]
    public void HeistBlueprint()
    {
        var actual = parser.ParseItem(@"Item Class: Blueprints
Rarity: Normal
Blueprint: Repository
--------
Area Level: 83
Wings Revealed: 1/3
Escape Routes Revealed: 1/6
Reward Rooms Revealed: 3/21
Requires Lockpicking (Level 1 (unmet))
Requires Demolition (Level 2 (unmet))
Requires Agility (Level 5 (unmet))
--------
Item Level: 83
--------
Use Intelligence to Reveal additional Wings and Rooms by talking to certain NPCs in the Rogue Harbour. Give this Blueprint to Adiyah to embark on the Grand Heist.
");

        Assert.Equal(ItemClass.HeistBlueprint, actual.Definition.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal(83, actual.Properties.AreaLevel);
        Assert.Equal(1, actual.Properties.HeistWingsRevealed);
        Assert.Equal(3, actual.Properties.HeistWingsTotal);
        Assert.Equal(1, actual.Properties.HeistRoutesRevealed);
        Assert.Equal(6, actual.Properties.HeistRoutesTotal);
        Assert.Equal(3, actual.Properties.HeistRoomsRevealed);
        Assert.Equal(21, actual.Properties.HeistRoomsTotal);
        Assert.Equal(1, actual.Properties.HeistLockpickingLevel);
        Assert.Equal(2, actual.Properties.HeistDemolitionLevel);
        Assert.Equal(5, actual.Properties.HeistAgilityLevel);
    }

    [Fact]
    public void HeistContract()
    {
        var actual = parser.ParseItem(@"Item Class: Contracts
Rarity: Normal
Contract: Records Office
--------
Client: Marcine Clavus
Heist Target: Enigmatic Assembly C5 (Moderate Value)
Area Level: 83
Requires Counter-Thaumaturgy (Level 5 (unmet))
--------
Item Level: 84
--------
""I have spent my family's entire fortune in the pursuit of this device.
Assembling it and turning it on is all I have left.""
--------
Give this Contract to Adiyah in the Rogue Harbour to embark on the Heist.
");

        Assert.Equal(ItemClass.HeistContract, actual.Definition.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal(HeistObjectiveValue.Moderate, actual.Properties.HeistObjectiveValue);
        Assert.Equal(5, actual.Properties.HeistCounterThaumaturgyLevel);
    }

}
