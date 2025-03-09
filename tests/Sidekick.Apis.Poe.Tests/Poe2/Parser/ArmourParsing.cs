using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe2.Parser;

[Collection(Collections.Poe2Parser)]
public class ArmourParsing(ParserFixture fixture)
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

        Assert.Equal(Category.Armour, actual.Header.Category);
        Assert.Equal(Rarity.Unique, actual.Header.Rarity);
        Assert.Equal("armour.boots", actual.Header.ItemCategory);
        Assert.Equal("Steeltoe Boots", actual.Header.ApiType);
        Assert.Equal("Thunderstep", actual.Header.ApiName);

        Assert.Equal(129, actual.Properties.EvasionRating);

        Assert.NotNull(actual.Properties.Sockets);
        Assert.Single(actual.Properties.Sockets);

        Assert.Equal(36, actual.Properties.ItemLevel);

        actual.AssertHasModifier(ModifierCategory.Enchant, "#% increased Evasion Rating", 22);
        actual.AssertHasModifier(ModifierCategory.Rune, "#% to Fire Resistance", 12);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Movement Speed", 10);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Evasion Rating", 41);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% to Maximum Lightning Resistance", 5);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% to Lightning Resistance", 33);

        Assert.True(actual.Properties.Corrupted);
    }
}
