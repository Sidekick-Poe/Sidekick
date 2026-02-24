using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe2English.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class ArmourParsing(Poe2EnglishFixture fixture)
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

        Assert.Equal(ItemClass.Boots, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Steeltoe Boots", actual.ApiInformation.Type);
        Assert.Equal("Thunderstep", actual.ApiInformation.Name);
        Assert.Equal("Thunderstep Steeltoe Boots", actual.ApiInformation.InvariantText);

        Assert.Equal(129, actual.Properties.EvasionRating);

        Assert.NotNull(actual.Properties.Sockets);
        Assert.Single(actual.Properties.Sockets);

        Assert.Equal(36, actual.Properties.ItemLevel);

        actual.AssertHasStat(StatCategory.Enchant, "#% increased Evasion Rating", 22);
        actual.AssertHasStat(StatCategory.Rune, "#% to Fire Resistance", 12);
        actual.AssertHasStat(StatCategory.Explicit, "#% increased Movement Speed", 10);
        actual.AssertHasStat(StatCategory.Explicit, "#% increased Evasion Rating", 41);
        actual.AssertHasStat(StatCategory.Explicit, "#% to Maximum Lightning Resistance", 5);
        actual.AssertHasStat(StatCategory.Explicit, "#% to Lightning Resistance", 33);

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

        Assert.Equal(ItemClass.Buckler, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Wooden Buckler", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);
        Assert.Equal(5, actual.Properties.RequiresLevel);
        Assert.Equal(11, actual.Properties.RequiresDexterity);

        Assert.Equal(16, actual.Properties.EvasionRating);

        Assert.Equal(5, actual.Properties.ItemLevel);

        actual.AssertHasStat(StatCategory.Explicit, "# to maximum Life", 12);
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

        Assert.Equal(ItemClass.Quiver, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Fire Quiver", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);

        Assert.Equal(8, actual.Properties.ItemLevel);
    }

    [Fact]
    public void ParseDesecratedBoots()
    {
        var actual = parser.ParseItem(
            @"Item Class: Boots
Rarity: Rare
Victory Hoof
Bastion Sabatons
--------
Quality: +20% (augmented)
Armour: 149 (augmented)
Evasion Rating: 118 (augmented)
--------
Requires: Level 59, 44 Str, 44 Dex
--------
Sockets: S 
--------
Item Level: 66
--------
+14% to Fire Resistance (rune)
--------
25% increased Movement Speed
+83 to maximum Life
+20% to Fire Resistance
+22% to Cold Resistance
+34% to Lightning Resistance
+32 to Armour (desecrated)
+14 to Evasion Rating (desecrated)
");

        Assert.Equal(ItemClass.Boots, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Bastion Sabatons", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);
        Assert.Equal(59, actual.Properties.RequiresLevel);
        Assert.Equal(44, actual.Properties.RequiresStrength);
        Assert.Equal(44, actual.Properties.RequiresDexterity);

        Assert.Equal(66, actual.Properties.ItemLevel);

        actual.AssertHasStat(StatCategory.Desecrated, "# to Armour", 32);
    }

    [Fact]
    public void ParseAtziriStep()
    {
        var actual = parser.ParseItem(@"Item Class: Boots
Rarity: Unique
Atziri's Step
Cinched Boots
--------
Quality: +20% (augmented)
Evasion Rating: 712 (augmented)
--------
Requires: Level 65, 86 Dex
--------
Sockets: S 
--------
Item Level: 83
--------
+21% to Fire Resistance (enchant)
+20% to Cold Resistance (enchant)
--------
18% increased Armour, Evasion and Energy Shield (rune)
--------
30% increased Movement Speed
111% increased Evasion Rating
+93 to Evasion Rating
Gain Deflection Rating equal to 60% of Evasion Rating
-9% to amount of Damage Prevented by Deflection
Cannot be Light Stunned by Deflected Hits — Unscalable Value
--------
""Those who dance are considered insane
by those who cannot hear the music.""
Atziri, Queen of the Vaal
--------
Corrupted
--------
Note: ~b/o 980 divine
");

        Assert.Equal(ItemClass.Boots, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Atziri's Step", actual.ApiInformation.Name);
        Assert.Equal("Cinched Boots", actual.ApiInformation.Type);

        actual.AssertHasStat(StatCategory.Explicit, "Gain Deflection Rating equal to #% of Evasion Rating", 60);
        // Issue #985 actual.AssertHasStat(StatCategory.Explicit, "#% to amount of Damage Prevented by Deflection", -9);
    }
}
