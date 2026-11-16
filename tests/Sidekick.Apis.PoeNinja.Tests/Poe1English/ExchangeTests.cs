using Sidekick.Game.Parser;
using Xunit;
namespace Sidekick.Apis.PoeNinja.Tests.Poe1English;

[Collection(Collections.NinjaPoe1TestCollection)]
public class ExchangeTests(NinjaPoe1TestFixture fixture)
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

        item.AssertHasNinjaExchange("chaos");
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

        item.AssertHasNinjaExchange("exalted");
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

        item.AssertHasNinjaExchange("the-void");
    }
}
