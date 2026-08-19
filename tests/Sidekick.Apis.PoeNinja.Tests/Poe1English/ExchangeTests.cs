using Sidekick.Game.Parser;
using Xunit;
namespace Sidekick.Apis.PoeNinja.Tests.Poe1English;

[Collection(Collections.NinjaTestCollection)]
public class ExchangeTests(NinjaTestFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void ChaosOrb()
    {
        var item = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Chaos Orb
--------
Stack Size: 7/20
--------
Reforges a rare item with new random modifiers
--------
Right click this item then left click a rare item to apply it.
Shift click to unstack.
");

        Assert.NotNull(item.InvariantDefinition?.NinjaExchangeItem);
        Assert.Equal("chaos", item.InvariantDefinition?.NinjaExchangeItem?.Id);
    }

    [Fact]
    public void ExaltedOrb()
    {
        var item = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Exalted Orb
--------
Stack Size: 1/20
--------
Augments a rare item with a new random modifier
--------
Right click this item then left click a rare item to apply it. Rare items can have up to six random modifiers.
");

        Assert.NotNull(item.InvariantDefinition?.NinjaExchangeItem);
        Assert.Equal("exalted", item.InvariantDefinition?.NinjaExchangeItem?.Id);
    }

    [Fact]
    public void TheVoid()
    {
        var item = parser.ParseItem(@"Item Class: Divination Cards
Rarity: Divination Card
The Void
--------
 
--------
Reach into the Void and claim your prize.
");

        Assert.NotNull(item.InvariantDefinition?.NinjaExchangeItem);
        Assert.Equal("the-void", item.InvariantDefinition?.NinjaExchangeItem?.Id);
    }
}
