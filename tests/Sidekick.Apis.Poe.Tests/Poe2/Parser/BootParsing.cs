using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe2.Parser;

[Collection(Collections.Poe2Parser)]
public class BootParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseThunderstep()
    {
        var actual = parser.ParseItem(
            @"Item Class: Boots
Rarity: Unique
Thunderstep
Steeltoe Boots
--------
Evasion Rating: 129 (augmented)
--------
Requirements:
Level: 28
Dex: 49
--------
Sockets: S 
--------
Item Level: 36
--------
22% increased Evasion Rating (enchant)
--------
+12% to Fire Resistance (rune)
--------
10% increased Movement Speed
41% increased Evasion Rating
+5% to Maximum Lightning Resistance
+33% to Lightning Resistance
--------
Where legends tread,
the world hearkens.
--------
Corrupted
");

        Assert.Equal(Category.Armour, actual.Metadata.Category);
        Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
        Assert.Equal(Class.Boots, actual.Header.Class);
        Assert.Equal("Steeltoe Boots", actual.Metadata.Type);
        Assert.Equal("Thunderstep", actual.Metadata.Name);

        Assert.Single(actual.Sockets);

        actual.AssertHasModifier(ModifierCategory.Enchant, "#% increased Evasion Rating", 22);
        actual.AssertHasModifier(ModifierCategory.Rune, "+#% to Fire Resistance", 12);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Evasion Rating", 41);
        // Ass
    }
}
