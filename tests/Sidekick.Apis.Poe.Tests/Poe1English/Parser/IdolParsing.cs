using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.Items.Models;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class IdolParsing(Poe1EnglishFixture fixture)
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

        Assert.Equal(ItemClass.Idol, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Null(actual.ApiInformation.Name);
        Assert.Equal("Minor Idol", actual.ApiInformation.Type);

        // actual.AssertHasModifier(ModifierCategory.Implicit, "#% increased Maps found in Area", 2);
        // actual.AssertHasModifier(ModifierCategory.Explicit, "Your Maps have #% chance to contain an additional Imprisoned Monster", 46);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Red Beasts in your Maps have #% increased chance to be from The Sands", 56);
    }

    [Fact]
    public void UniqueIdol()
    {
        var actual = parser.ParseItem(@"Item Class: Idols
Rarity: Unique
Loved by the Sun
Minor Idol
--------
Limited to: 1
--------
Item Level: 70
--------
2% increased Maps found in Area (implicit)
--------
Your Maps have no chance to contain Abysses
Scarabs found in your Maps cannot be Abyss Scarabs
Your Maps have +5% chance to contain other Extra Content that can
be turned off through Atlas Passives
--------
Place this item into the Idol inventory at a Map Device to affect Maps you open. Idols are not consumed when opening Maps.
--------
Unmodifiable
");

        Assert.Equal(ItemClass.Idol, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Loved by the Sun", actual.ApiInformation.Name);
        Assert.Equal("Minor Idol", actual.ApiInformation.Type);

        // actual.AssertHasModifier(ModifierCategory.Implicit, "#% increased Maps found in Area", 2);
        // actual.AssertHasModifier(ModifierCategory.Explicit, "Your Maps have #% chance to contain an additional Imprisoned Monster", 46);
        // actual.AssertHasModifier(ModifierCategory.Explicit, "Red Beasts in your Maps have 56% increased chance to be from The Sands", 56);
    }
}
