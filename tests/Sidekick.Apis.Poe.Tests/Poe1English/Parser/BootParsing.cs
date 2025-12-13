using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class BootParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseFracturedItem()
    {
        var actual = parser.ParseItem(@"Item Class: Boots
Rarity: Rare
Invasion Track
Iron Greaves
--------
Armour: 6
--------
Sockets: B B
--------
Item Level: 2
--------
10% increased Movement Speed (fractured)
+5 to maximum Life
Regenerate 1.9 Life per second
+8% to Cold Resistance
--------
Fractured Item
");

        Assert.Equal(ItemClass.Boots, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Iron Greaves", actual.ApiInformation.Type);

        actual.AssertHasModifier(ModifierCategory.Fractured, "#% increased Movement Speed", 10);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Movement Speed", 10);
    }
}
