using Sidekick.Common.Game.Items;
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

        Assert.Equal(Category.Jewel, actual.Metadata.Category);
        Assert.Equal("Emerald", actual.Metadata.Type);
        Assert.Null(actual.Metadata.Name);
        Assert.Equal(26, actual.Properties.ItemLevel);

        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Attack Speed", 3);
        actual.AssertHasModifier(ModifierCategory.Explicit, "Damage Penetrates #% Lightning Resistance", 5);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Magnitude of Ailments you inflict", 7);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Pin Buildup", 14);
    }
}
