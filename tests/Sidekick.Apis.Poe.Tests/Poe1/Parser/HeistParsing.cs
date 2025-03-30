using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class HeistParsing(ParserFixture fixture)
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

        Assert.Equal("heistequipment.heisttool", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Magic, actual.Header.Rarity);
        Assert.Equal(Category.HeistEquipment, actual.Header.Category);
        Assert.Equal("Basic Disguise Kit", actual.Header.ApiType);
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

        Assert.Equal("heistequipment.heistutility", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Normal, actual.Header.Rarity);
        Assert.Equal(Category.HeistEquipment, actual.Header.Category);
        Assert.Equal("Torn Cloak", actual.Header.ApiType);
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

        Assert.Equal("heistequipment.heistreward", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Normal, actual.Header.Rarity);
        Assert.Equal(Category.HeistEquipment, actual.Header.Category);
        Assert.Equal("Silver Brooch", actual.Header.ApiType);
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

        Assert.Equal("heistequipment.heistweapon", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Rare, actual.Header.Rarity);
        Assert.Equal(Category.HeistEquipment, actual.Header.Category);
        Assert.Equal("Rough Sharpening Stone", actual.Header.ApiType);
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

        Assert.Equal("currency.heistobjective", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Currency, actual.Header.Rarity);
        Assert.Equal(Category.Currency, actual.Header.Category);
        Assert.Equal("Golden Napuatzi Idol", actual.Header.ApiType);
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

        Assert.Equal("accessory.trinket", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Rare, actual.Header.Rarity);
        Assert.Equal(Category.Accessory, actual.Header.Category);
        Assert.Equal("Thief's Trinket", actual.Header.ApiType);
        Assert.True(actual.Properties.Corrupted);
    }

}
