using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.ItemClasses;
using Sidekick.Data.Items;
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

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Essence of Enhancement", actual.Definition.TradeItem?.Type);
        Assert.Null(actual.Definition.TradeItem?.Name);
    }

    [Fact]
    public void ParseAlloy()
    {
        var actual = parser.ParseItem(
            @"Item Class: Stackable Currency
Rarity: Currency
Swift Alloy
--------
Stack Size: 6/10
--------
Removes a random modifier and augments a Rare item with a new guaranteed modifier
Gloves: (9-12)% increased Cast Speed
Ring: (7-9)% increased Attack Speed
Belt: Flasks gain (0.75-1) charges per Second
Shield or Focus: (30-49)% increased Totem Placement speed
--------
Right click this item then left click a Rare item to apply it.
Shift click to unstack.");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Swift Alloy", actual.Definition.TradeItem?.Type);
        Assert.Null(actual.Definition.TradeItem?.Name);
    }
}
