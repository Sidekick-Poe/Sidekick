using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
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

        Assert.Equal(ItemClass.Currency, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Chaos Orb", actual.Definition.Type);
        Assert.Equal("chaos", actual.Definition.Id);

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

        Assert.Equal(ItemClass.Currency, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Exalted Orb", actual.Definition.Type);
        Assert.Equal("exalted", actual.Definition.Id);

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

        Assert.Equal(ItemClass.Currency, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Clear Oil", actual.Definition.Type);
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

        Assert.Equal(ItemClass.Currency, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Crystallised Rancour", actual.Definition.Type);
    }

}
