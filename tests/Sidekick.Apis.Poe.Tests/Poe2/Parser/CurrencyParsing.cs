using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe2.Parser;

[Collection(Collections.Poe2Parser)]
public class CurrencyParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseEssence()
    {
        var actual = parser.ParseItem(
            @"Item Class: Stackable Currency
Rarity: Currency
Essence of Enhancement
--------
Stack Size: 1/10
--------
Upgrades a normal item to Magic with one Defence modifier
--------
Right click this item then left click a normal item to apply it.
");

        Assert.Equal(Category.Currency, actual.Metadata.Category);
        Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
        Assert.Equal("currency", actual.Header.ItemCategory);
        Assert.Equal("Essence of Enhancement", actual.Metadata.Type);
        Assert.Null(actual.Metadata.Name);
    }
}
