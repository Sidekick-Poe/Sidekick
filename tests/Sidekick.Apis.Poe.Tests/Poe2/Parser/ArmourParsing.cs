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
        Assert.Equal("armour.boots", actual.Header.ApiItemCategory);
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

    [Fact]
    public void ParseBuckler()
    {
        var actual = parser.ParseItem(
            @"Item Class: Bucklers
Rarity: Magic
Hale Wooden Buckler
--------
Block chance: 20%
Evasion Rating: 16
--------
Requires: Level 5, 11 Dex
--------
Item Level: 5
--------
Grants Skill: Parry
--------
+12 to maximum Life

");

        Assert.Equal(Category.Armour, actual.Header.Category);
        Assert.Equal(Rarity.Magic, actual.Header.Rarity);
        Assert.Equal("armour.buckler", actual.Header.ApiItemCategory);
        Assert.Equal("Wooden Buckler", actual.Header.ApiType);
        Assert.Null(actual.Header.ApiName);

        Assert.Equal(16, actual.Properties.EvasionRating);

        Assert.Equal(5, actual.Properties.ItemLevel);

        actual.AssertHasModifier(ModifierCategory.Explicit, "# to maximum Life", 12);
    }

    [Fact]
    public void ParseUnusableQuiver()
    {
        var actual = parser.ParseItem(
            @"Item Class: Quivers
Rarity: Normal
You cannot use this item. Its stats will be ignored
--------
Fire Quiver
--------
Requires: Level 8
--------
Item Level: 8
--------
Adds 3 to 5 Fire damage to Attacks (implicit)
--------
Can only be equipped if you are wielding a Bow.
");

        Assert.Equal(Category.Armour, actual.Header.Category);
        Assert.Equal(Rarity.Normal, actual.Header.Rarity);
        Assert.Equal("armour.quiver", actual.Header.ApiItemCategory);
        Assert.Equal("Fire Quiver", actual.Header.ApiType);
        Assert.Null(actual.Header.ApiName);

        Assert.Equal(8, actual.Properties.ItemLevel);
    }
}
