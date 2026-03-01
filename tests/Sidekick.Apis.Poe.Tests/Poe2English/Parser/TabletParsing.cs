using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.Items;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe2English.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class TabletParsing(Poe2EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseTablet()
    {
        var actual = parser.ParseItem(
            @"Item Class: Tablet
Rarity: Magic
Bountiful Overseer Precursor Tablet of Conquering
--------
Item Level: 82
--------
Empowers the Map Boss of a Map (implicit)
10 uses remaining (implicit)
--------
33% increased Gold found in Map
Map Bosses grant 53% increased Experience
--------
Can be used in a personal Map Device to add modifiers to a Map.");

        Assert.Equal(ItemClass.Tablet, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Overseer Precursor Tablet", actual.ApiInformation.Type);

        Assert.Equal(82, actual.Properties.ItemLevel);

        fixture.AssertHasStat(actual, StatCategory.Implicit, "Empowers the Map Boss of a Map \n# use remaining", 10);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Gold found in your Maps (Gold Piles)", 33);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Map Bosses grant #% increased Experience", 53);
    }

    [Fact]
    public void ParseReducedTributeTablet()
    {
        var actual = parser.ParseItem(
            @"Item Class: Tablet
Rarity: Magic
Ritual Precursor Tablet
--------
Item Level: 77
--------
Empowers the Map Boss of a Map (implicit)
10 uses remaining (implicit)
--------
Rerolling Favours at Ritual Altars in Map costs 28% reduced Tribute
Fortifying Hits grant 25% reduced Fortification
--------
Can be used in a personal Map Device to add modifiers to a Map.");

        Assert.Equal(ItemClass.Tablet, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Ritual Precursor Tablet", actual.ApiInformation.Type);

        Assert.Equal(77, actual.Properties.ItemLevel);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "Rerolling Favours at Ritual Altars in your Maps costs #% increased Tribute", -28);
        actual.AssertDoesNotHaveModifier(StatCategory.Explicit, "Deferring Favours at Ritual Altars in Area costs #% reduced Tribute");
        actual.AssertDoesNotHaveModifier(StatCategory.Explicit, "Deferring Favours at Ritual Altars in your Maps costs #% reduced Tribute");
    }
}
