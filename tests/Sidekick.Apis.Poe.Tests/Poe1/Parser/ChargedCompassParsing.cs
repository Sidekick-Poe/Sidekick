using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class ChargedCompassParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseNikoModifier()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Normal
Charged Compass
--------
Item Level: 69
--------
Your Maps contain Niko (enchant)
3 uses remaining (enchant)
--------
Right click on this item then left click on a Voidstone to apply the itemised Sextant Modifier to the Voidstone.
");

        Assert.Equal(ItemClass.Currency, actual.Properties.ItemClass);
        Assert.Equal(Category.Currency, actual.ApiInformation.Category);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);

        actual.AssertHasModifier(ModifierCategory.Enchant, "Your Maps contain Niko (Master)");
    }
}
