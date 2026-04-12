using System.Threading.Tasks;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.PoeNinja.Tests.Poe1English;

[Collection(Collections.NinjaTestCollection)]
public class ExchangeTests(NinjaTestFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public async Task ChaosOrb()
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

        var result = await fixture.NinjaExchangeProvider.GetInfo(item.Invariant);

        Assert.NotNull(result);
        Assert.Equal("chaos", result.Id);
        Assert.Equal(2, result.Trades.Count);
        Assert.Equal("chaos", result.Trades[0].ExchangeId);
        Assert.Equal("divine", result.Trades[1].ExchangeId);
    }

    [Fact]
    public async Task ExaltedOrb()
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

        var result = await fixture.NinjaExchangeProvider.GetInfo(item.Invariant);

        Assert.NotNull(result);
        Assert.Equal("exalted", result.Id);
    }

    [Fact]
    public async Task TheVoid()
    {
        var item = parser.ParseItem(@"Item Class: Divination Cards
Rarity: Divination Card
The Void
--------
 
--------
Reach into the Void and claim your prize.
");

        var result = await fixture.NinjaExchangeProvider.GetInfo(item.Invariant);

        Assert.NotNull(result);
        Assert.Equal("the-void", result.Id);
    }
}
