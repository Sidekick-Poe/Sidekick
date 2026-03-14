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
    public void ParseRitualTablet()
    {
        var actual = parser.ParseItem(
            @"Item Class: Tablet
Rarity: Rare
Planar Injunction
Ritual Precursor Tablet
--------
Item Level: 77
--------
Adds Ritual Altars to a Map (implicit)
10 uses remaining (implicit)
--------
18% increased Rarity of Items found in Map
Map has 40% increased number of Rare Monsters
Monsters Sacrificed at Ritual Altars in Map grant 18% increased Tribute
Deferring Favours at Ritual Altars in Map costs 25% reduced Tribute
--------
Can be used in a personal Map Device to add modifiers to a Map.
");

        Assert.Equal(ItemClass.Tablet, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Ritual Precursor Tablet", actual.ApiInformation.Type);

        Assert.Equal(77, actual.Properties.ItemLevel);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Rarity of Items found in your Maps",18);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased number of Rare Monsters", 40);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Monsters Sacrificed at Ritual Altars in Area grant #% increased Tribute", 18);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Deferring Favours at Ritual Altars in Area costs #% increased Tribute", -25);
    }
}
