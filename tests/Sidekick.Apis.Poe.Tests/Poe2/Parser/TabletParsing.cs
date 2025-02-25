using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe2.Parser;

[Collection(Collections.Poe2Parser)]
public class TabletParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseTablet()
    {
        var actual = parser.ParseItem(
            @"Item Class: Tablet
Rarity: Magic
Teeming Precursor Tablet of the Essence
--------
Item Level: 69
--------
9 Maps in Range are Irradiated (implicit)
--------
Your Maps have +10% chance to contain an Essence
22% increased Magic Monsters in your Maps
--------
Can be used in a completed Tower on your Atlas to influence surrounding Maps. Tablets are consumed once placed into a Tower.");

        Assert.Equal(Category.Map, actual.Header.Category);
        Assert.Equal(Rarity.Magic, actual.Header.Rarity);
        Assert.Equal("map.tablet", actual.Header.ItemCategory);
        Assert.Equal("Precursor Tablet", actual.Header.ApiType);
        Assert.Null(actual.Header.ApiName);

        actual.AssertHasModifier(ModifierCategory.Implicit, "# Maps in Range are Irradiated", 9);
        actual.AssertHasModifier(ModifierCategory.Explicit, "Area has #% chance to contain an Essence", 10);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Magic Monsters", 22);
    }
}
