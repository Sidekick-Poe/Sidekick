using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe2English.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class FragmentParsing(Poe2EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseWaystone()
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

        actual.AssertHasStat(StatCategory.Implicit, "Empowers the Map Boss of a Map \n# use remaining", 10);
        actual.AssertHasStat(StatCategory.Explicit, "#% increased Gold found in your Maps", 33);
        actual.AssertHasStat(StatCategory.Explicit, "Map Bosses grant #% increased Experience", 53);
    }
}
