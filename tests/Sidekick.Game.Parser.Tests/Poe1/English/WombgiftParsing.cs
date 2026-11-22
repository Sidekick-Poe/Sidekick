using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe1.English;

[Collection(Collections.Poe1EnglishFixture)]
public class WombgiftParsing(Poe1EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void ProvisioningWombgift()
    {
        var actual = parser.ParseItem(@"Item Class: Wombgifts
Rarity: Currency
Provisioning Wombgift
--------
Item Level: 83
Requires 1239 Hiveblood
--------
Can grow into an Armour or Jewellery item on the Genesis Tree
--------
Place this item into an allocated equipment item womb on the Genesis Tree. Right click to retrieve from the Genesis Tree.
");

        Assert.Equal(ItemClass.Wombgift, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Provisioning Wombgift", actual.TradeItem?.Type);

        Assert.Equal(83, actual.Properties.ItemLevel);
        Assert.True(actual.InvariantDefinition?.SupportsTrade);

        actual.AssertHasNinjaStash("provisioning-wombgift-75");
    }
}
