using System.Threading.Tasks;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.PoeNinja.Tests.Poe1English;

[Collection(Collections.NinjaTestCollection)]
public class ExchangeTests(NinjaTestFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

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
}
