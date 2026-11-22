using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe1.English;

[Collection(Collections.Poe1EnglishFixture)]
public class CorpseParsing(Poe1EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void PerfectNeedleHorror()
    {
        var actual = parser.ParseItem(@"Item Class: Corpses
Rarity: Currency
Perfect Needle Horror
--------
Corpse Level: 83
Monster Category: Demon
--------
Item Level: 83
--------
Throws Physical Projectiles (implicit)
100% increased Impale Effect (implicit)
Owner gains 10% increased Impale Effect (implicit)
--------
Right click this item to create this corpse.
--------
Note: ~price 3 chaos
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Perfect Needle Horror", actual.TradeItem?.Type);
    }
}
