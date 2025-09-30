using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class OrbParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ChaosOrb()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Chaos Orb
--------
Stack Size: 1/10
--------
Reforges a rare item with new random modifiers
--------
Right click this item then left click a rare item to apply it.
--------
Note: ~b/o 2 blessed
");

        Assert.Equal(ItemClass.Currency, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal(Category.Currency, actual.ApiInformation.Category);
        Assert.Equal("Chaos Orb", actual.ApiInformation.Type);

        Assert.Empty(actual.ModifierLines);
    }
}
