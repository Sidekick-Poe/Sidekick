using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe2English.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class JewelParsing(Poe2EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseEmerald()
    {
        var actual = parser.ParseItem(
            @"Item Class: Jewels
Rarity: Rare
Soul Bliss
Emerald
--------
Item Level: 26
--------
3% increased Attack Speed
Damage Penetrates 5% Lightning Resistance
7% increased Magnitude of Ailments you inflict
14% increased Pin Buildup
--------
Place into an allocated Jewel Socket on the Passive Skill Tree. Right click to remove from the Socket.
");

        Assert.Equal(ItemClass.Jewel, actual.Properties.ItemClass);
        Assert.Equal("Emerald", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);
        Assert.Equal(26, actual.Properties.ItemLevel);

        actual.AssertHasStat(StatCategory.Explicit, "#% increased Attack Speed", 3);
        actual.AssertHasStat(StatCategory.Explicit, "Damage Penetrates #% Lightning Resistance", 5);
        actual.AssertHasStat(StatCategory.Explicit, "#% increased Magnitude of Ailments you inflict", 7);
        actual.AssertHasStat(StatCategory.Explicit, "#% increased Pin Buildup", 14);
    }

    [Fact]
    public void ParseTimeLost()
    {
        var actual = parser.ParseItem(
            @"Item Class: Jewels
Rarity: Rare
Fulgent Shard
Time-Lost Ruby
--------
Radius: Small
--------
Item Level: 64
--------
Small Passive Skills in Radius also grant 2% increased Totem Damage
Notable Passive Skills in Radius also grant 4% increased Magnitude of Bleeding you inflict
Notable Passive Skills in Radius also grant 7% increased Stun Buildup with Maces
--------
Place into an allocated Jewel Socket on the Passive Skill Tree. Right click to remove from the Socket.");

        Assert.Equal(ItemClass.Jewel, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Time-Lost Ruby", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);
        Assert.Equal(64, actual.Properties.ItemLevel);
    }

    [Fact]
    public void ParseDragonIchor()
    {
        var actual = parser.ParseItem(
            @"Item Class: Jewels
Rarity: Rare
Dragon Ichor
Emerald
--------
Item Level: 24
--------
8% increased Damage with Crossbows
9% increased Duration of Ignite, Shock and Chill on Enemies
7% increased Magnitude of Poison you inflict
13% increased Crossbow Reload Speed
--------
Place into an allocated Jewel Socket on the Passive Skill Tree. Right click to remove from the Socket.");

        Assert.Equal(ItemClass.Jewel, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Emerald", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);
        Assert.Equal(24, actual.Properties.ItemLevel);

        actual.AssertHasStat(StatCategory.Explicit, "#% increased Damage with Crossbows", 8);
        actual.AssertHasStat(StatCategory.Explicit, "#% increased Duration of Ignite, Shock and Chill on Enemies", 9);
        actual.AssertHasStat(StatCategory.Explicit, "#% increased Magnitude of Poison you inflict", 7);
        actual.AssertHasStat(StatCategory.Explicit, "#% increased Crossbow Reload Speed", 13);
    }
}
