using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class IdolParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void MinorIdol()
    {
        var actual = parser.ParseItem(@"Item Class: Idols
Rarity: Magic
Essence Minor Idol of Beasts
--------
Item Level: 68
--------
2% increased Maps found in Area (implicit)
--------
Your Maps have 46% chance to contain an additional Imprisoned Monster
Red Beasts in your Maps have 56% increased chance to be from The Sands
--------
Place this item into the Idol inventory at a Map Device to affect Maps you open. Idols are not consumed when opening Maps.
--------
Unmodifiable");

        Assert.Equal("idol", actual.Header.ItemCategory);
        Assert.Equal(Category.Idol, actual.Header.Category);
        Assert.Equal(Rarity.Magic, actual.Header.Rarity);
        Assert.Null(actual.Header.ApiName);
        Assert.Equal("Minor Idol", actual.Header.ApiType);

        actual.AssertHasModifier(ModifierCategory.Implicit, "#% increased Maps found in Area", 2);
        // actual.AssertHasModifier(ModifierCategory.Explicit, "Your Maps have #% chance to contain an additional Imprisoned Monster", 46);
        actual.AssertHasModifier(ModifierCategory.Explicit, "Red Beasts in your Maps have 56% increased chance to be from The Sands", 56);
    }
}
