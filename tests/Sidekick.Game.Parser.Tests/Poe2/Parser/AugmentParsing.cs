using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe2.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class AugmentParsing(Poe2EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

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
}
