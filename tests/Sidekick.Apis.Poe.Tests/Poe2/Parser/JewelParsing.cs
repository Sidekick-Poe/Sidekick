using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe2.Parser;

[Collection(Collections.Poe2Parser)]
public class JewelParsing(ParserFixture fixture)
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

        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Attack Speed", 3);
        actual.AssertHasModifier(ModifierCategory.Explicit, "Damage Penetrates #% Lightning Resistance", 5);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Magnitude of Ailments you inflict", 7);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Pin Buildup", 14);
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
}
