using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe2.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class CurrencyParsing(Poe2EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

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
        Assert.Equal("Essence of Enhancement", actual.TradeItem?.Type);
        Assert.Null(actual.TradeItem?.Name);
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
        Assert.Equal("Swift Alloy", actual.TradeItem?.Type);
        Assert.Null(actual.TradeItem?.Name);
    }

    [Fact]
    public void VoranasCarnage()
    {
        var actual = parser.ParseItem(@"Item Class: Augment
Rarity: Currency
Vorana's Carnage
--------
Stack Size: 2/10
Rune
Limited to: 1
--------
Socket-bound (rune)
--------
Helmets: Can roll Berserking modifiers
--------
Vorana the Irrepressible never once recruited. The Black
Scythe Mercenaries were entirely composed of men and
women who had seen her relentless will to fight - and
thought to themselves, ""I would follow her anywhere.""
--------
Place into an empty Augment Socket in a Helmet to apply its effect to that item. Once socketed it cannot be retrieved or replaced.
Shift click to unstack.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Vorana's Carnage", actual.TradeItem?.Type);
        actual.AssertHasNinjaExchange("voranas-carnage");
    }

    [Fact]
    public void SacredBloom()
    {
        var actual = parser.ParseItem(@"Item Class: Map Fragments
Rarity: Currency
Sacred Bloom
--------
Adds a Viridian Wildwood to a Map and all Maps connected to it
--------
It neither blooms nor withers.
--------
Right click this item then left click an Endgame World Area to apply it.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Sacred Bloom", actual.TradeItem?.Type);
        actual.AssertHasNinjaExchange("sacred-bloom");
    }
}
