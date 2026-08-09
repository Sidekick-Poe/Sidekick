using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Game.ItemClasses;
using Sidekick.Game.Items;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class CurrencyParsing(Poe1EnglishFixture fixture)
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

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Chaos Orb", actual.TradeItem?.Type);
        Assert.Equal("chaos", actual.Exchange?.Id);
        Assert.Equal("chaos", actual.NinjaExchange?.Id);

        Assert.Empty(actual.Stats);
    }

    [Fact]
    public void ExaltedOrb()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Exalted Orb
--------
Stack Size: 1/20
--------
Augments a rare item with a new random modifier
--------
Right click this item then left click a rare item to apply it. Rare items can have up to six random modifiers.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Exalted Orb", actual.TradeItem?.Type);
        Assert.Equal("exalted", actual.Exchange?.Id);
        Assert.Equal("exalted", actual.NinjaExchange?.Id);

        Assert.Empty(actual.Stats);
    }

    [Fact]
    public void ClearOil()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Clear Oil
--------
Stack Size: 5/10
--------
Can be combined with other Oils at Cassia to Enchant Rings or Amulets, or to modify Blighted Maps.
Shift click to unstack.
--------
Note: ~price 1 blessed
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Clear Oil", actual.TradeItem?.Type);
    }

    [Fact]
    public void CrystallisedRancour()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Crystallised Rancour
--------
Stack Size: 2/10
--------
Can be used at the Horticrafting bench in your hideout.
Shift click to unstack.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Crystallised Rancour", actual.TradeItem?.Type);
    }

    [Fact]
    public void TrarthanScarabOfInfamy()
    {
        var actual = parser.ParseItem(@"Item Class: Map Fragments
Rarity: Normal
Trarthan Scarab of Infamy
--------
Stack Size: 1/20
Limit: 1
--------
Mercenaries found in Area are Infamous
Mercenaries found in Area are accompanied by two Wild Mercenaries
--------
Some men must make their own way.
--------
Can be used in a personal Map Device to add modifiers to a Map.
");

        Assert.Equal(ItemClass.MapFragments, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Trarthan Scarab of Infamy", actual.TradeItem?.Type);
    }

    [Fact]
    public void Ducat()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
The Changeling's Ducat
--------
Stack Size: 1/10
--------
Transforms into a random Ducat
--------
To appease Tsoatha, they tossed it to sea, but the storm grew ever madder.
None returned to shore. Yet the vessel sails on, and its captain is merry.
--------
Can be used as part of Allflame Crafting aboard The Sovereign.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("The Changeling's Ducat", actual.TradeItem?.Type);
    }
}
