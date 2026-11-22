using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Stats;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe2.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class OffhandParsing(Poe2EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void HaleWoodenBuckler()
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

        Assert.Equal(ItemClass.Buckler, actual.ItemClass.Type);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Wooden Buckler", actual.TradeItem?.Type);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal(5, actual.Properties.RequiresLevel);
        Assert.Equal(11, actual.Properties.RequiresDexterity);

        Assert.Equal(16, actual.Properties.EvasionRating);

        Assert.Equal(5, actual.Properties.ItemLevel);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "# to maximum Life", 12);
    }

    [Fact]
    public void FireQuiver()
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

        Assert.Equal(ItemClass.Quiver, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Fire Quiver", actual.TradeItem?.Type);
        Assert.Null(actual.TradeItem?.Name);

        Assert.Equal(8, actual.Properties.ItemLevel);
    }
}
