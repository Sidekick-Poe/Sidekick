using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class UltimatumParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void NoxiousCatalyst()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Noxious Catalyst
--------
Stack Size: 1/10
--------
Adds quality that enhances Physical and Chaos Damage modifiers on a ring, amulet or belt
Replaces other quality types
--------
Right click this item then left click a ring, amulet or belt to apply it. Has greater effect on lower-rarity jewellery. The maximum quality is 20%.
");

        Assert.Equal(ItemClass.Currency, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Noxious Catalyst", actual.ApiInformation.Type);
    }
}
