using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe2English.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class CurrencyParsing(Poe2EnglishFixture fixture)
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

        Assert.Equal(ItemClass.Currency, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Essence of Enhancement", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);
    }
}
