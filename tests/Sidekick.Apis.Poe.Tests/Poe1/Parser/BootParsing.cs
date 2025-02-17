using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class BootParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseFracturedItem()
    {
        var actual = parser.ParseItem(@"Item Class: Unknown
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

        Assert.Equal(Category.Armour, actual.Header.Category);
        Assert.Equal(Rarity.Rare, actual.Header.Rarity);
        Assert.Equal("Iron Greaves", actual.Header.ApiType);

        actual.AssertHasModifier(ModifierCategory.Fractured, "#% increased Movement Speed", 10);
    }
}
