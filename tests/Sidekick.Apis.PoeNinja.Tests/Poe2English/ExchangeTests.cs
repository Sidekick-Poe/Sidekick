using Sidekick.Game.Parser;
using Xunit;
namespace Sidekick.Apis.PoeNinja.Tests.Poe2English;

[Collection(Collections.NinjaPoe2TestCollection)]
public class ExchangeTests(NinjaPoe2TestFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void VoranasCarnage()
    {
        var item = parser.ParseItem(@"Item Class: Augment
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

        Assert.NotNull(item.InvariantDefinition?.NinjaExchangeItem);
        Assert.Equal("voranas-carnage", item.InvariantDefinition?.NinjaExchangeItem?.Id);
    }
}
