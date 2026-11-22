using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe2.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class MapParsing(Poe2EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

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
