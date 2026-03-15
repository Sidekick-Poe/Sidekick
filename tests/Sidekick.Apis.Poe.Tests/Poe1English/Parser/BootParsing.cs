using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.Stats;
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
        Assert.Equal("Iron Greaves", actual.Definition.Type);

        fixture.AssertHasStat(actual, StatCategory.Fractured, "#% increased Movement Speed", 10);
    }

    [Fact]
    public void BulbonicTrail()
    {
        var actual = parser.ParseItem(@"Item Class: Boots
Rarity: Unique
Bubonic Trail
Murder Boots
--------
Evasion Rating: 185
Energy Shield: 17
--------
Requirements:
Level: 69
Dex: 82
Int: 42
--------
Sockets: G-G A
--------
Item Level: 84
--------
Has 1 Abyssal Socket
Triggers Level 20 Death Walk when Equipped
6% increased maximum Life
30% increased Movement Speed
10% increased Damage for each type of Abyss Jewel affecting you
--------
Even the dead serve the Lightless.
");

        Assert.Equal(ItemClass.Boots, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Bubonic Trail", actual.Definition.Name);
        Assert.Equal("Murder Boots", actual.Definition.Type);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "Has # Abyssal Sockets", 1);
    }
}
