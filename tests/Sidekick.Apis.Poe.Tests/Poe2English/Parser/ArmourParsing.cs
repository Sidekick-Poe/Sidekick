using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.ItemClasses;
using Sidekick.Data.Items;
using Sidekick.Data.Stats;
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

        Assert.Equal(ItemClass.Boots, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Steeltoe Boots", actual.Definition.TradeItem?.Type);
        Assert.Equal("Thunderstep", actual.Definition.TradeItem?.Name);
        Assert.Equal("Thunderstep Steeltoe Boots", actual.Definition.TradeItem?.Text);

        Assert.Equal(129, actual.Properties.EvasionRating);

        Assert.NotNull(actual.Properties.Sockets);
        Assert.Single(actual.Properties.Sockets);

        Assert.Equal(36, actual.Properties.ItemLevel);

        fixture.AssertHasStat(actual, StatCategory.Enchant, "#% increased Evasion Rating", 22);
        fixture.AssertHasStat(actual, StatCategory.Rune, "#% to Fire Resistance", 12);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Movement Speed", 10);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Evasion Rating", 41);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% to Maximum Lightning Resistance", 5);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% to Lightning Resistance", 33);

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

        Assert.Equal(ItemClass.Buckler, actual.ItemClass.Type);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Wooden Buckler", actual.Definition.TradeItem?.Type);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal(5, actual.Properties.RequiresLevel);
        Assert.Equal(11, actual.Properties.RequiresDexterity);

        Assert.Equal(16, actual.Properties.EvasionRating);

        Assert.Equal(5, actual.Properties.ItemLevel);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "# to maximum Life", 12);
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

        Assert.Equal(ItemClass.Quiver, actual.ItemClass.Type);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal("Fire Quiver", actual.Definition.TradeItem?.Type);
        Assert.Null(actual.Definition.TradeItem?.Name);

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

        Assert.Equal(ItemClass.Boots, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Bastion Sabatons", actual.Definition.TradeItem?.Type);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal(59, actual.Properties.RequiresLevel);
        Assert.Equal(44, actual.Properties.RequiresStrength);
        Assert.Equal(44, actual.Properties.RequiresDexterity);

        Assert.Equal(66, actual.Properties.ItemLevel);

        fixture.AssertHasStat(actual, StatCategory.Desecrated, "# to Armour", 32);
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

        Assert.Equal(ItemClass.Boots, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Atziri's Step", actual.Definition.TradeItem?.Name);
        Assert.Equal("Cinched Boots", actual.Definition.TradeItem?.Type);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "Gain Deflection Rating equal to #% of Evasion Rating", 60);
        // Issue #985 fixture.AssertHasStat(actual, StatCategory.Explicit, "#% to amount of Damage Prevented by Deflection", -9);
    }

    [Fact]
    public void ParsePainGuardian()
    {
        var actual = parser.ParseItem(@"Item Class: Body Armours
Rarity: Rare
Pain Guardian
Sacramental Robe
--------
Quality: +20% (augmented)
Energy Shield: 600 (augmented)
Runic Ward: 30 (augmented)
--------
Requires: Level 75, 84 (augmented) Int
--------
Sockets: S S 
--------
Item Level: 79
--------
+25 to maximum Runic Ward (rune)
18% increased Armour, Evasion and Energy Shield (rune)
--------
{ Implicit Modifier — Energy Shield }
20(20-25)% increased Energy Shield Recharge Rate
--------
{ Desecrated Prefix Modifier ""Pope's"" (Tier: 1) — Life, Energy Shield }
42(39-42)% increased Energy Shield
+46(42-49) to maximum Life
{ Prefix Modifier ""Archon's"" (Tier: 2) — Energy Shield }
+24(21-25) to maximum Energy Shield
37(33-38)% increased Energy Shield
{ Prefix Modifier ""Scintillating"" (Tier: 3) — Energy Shield }
+77(74-80) to maximum Energy Shield
{ Suffix Modifier ""of the Starfish"" (Tier: 8) — Life }
4.7(4.1-6) Life Regeneration per second
{ Suffix Modifier ""of the Skilled"" (Tier: 2) }
30% reduced Attribute Requirements
{ Crafted Suffix Modifier ""of the Essence"" }
Hits against you have 43(40-50)% reduced Critical Damage Bonus
");

        Assert.Equal(ItemClass.BodyArmour, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Sacramental Robe", actual.Definition.TradeItem?.Type);

        Assert.Equal(79, actual.Properties.ItemLevel);
        Assert.Equal(600, actual.Properties.EnergyShield);
        // TODO Runic Ward
        // Assert.Equal(30, actual.Properties.RunicWard);

        Assert.Equal(75, actual.Properties.RequiresLevel);
        Assert.Equal(84, actual.Properties.RequiresIntelligence);

        fixture.AssertHasStat(actual, StatCategory.Implicit, "#% increased Energy Shield Recharge Rate", 20);
        fixture.AssertHasStat(actual, StatCategory.Desecrated, "#% increased Energy Shield", 42);
        fixture.AssertHasStat(actual, StatCategory.Desecrated, "# to maximum Life", 46);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "# to maximum Energy Shield", 101);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Energy Shield", 37);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "# Life Regeneration per second", 4.7);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% reduced Attribute Requirements", -30);
        fixture.AssertHasStat(actual, StatCategory.Crafted, "Hits against you have #% reduced Critical Damage Bonus", 43);
    }

    [Fact]
    public void ParseIronride()
    {
        var actual = parser.ParseItem(@"Item Class: Helmets
Rarity: Unique
Ironride
Visored Helm
--------
Quality: +20% (augmented)
Armour: 105 (augmented)
Evasion Rating: 89 (augmented)
--------
Requires: Level 16, 14 (augmented) Str, 14 (augmented) Dex
--------
Sockets: S 
--------
Item Level: 81
--------
{ Corruption Enhancement — Mana }
30(20-30)% increased Mana Regeneration Rate
--------
{ Unique Modifier — Armour, Evasion }
62(60-80)% increased Armour and Evasion
{ Unique Modifier — Elemental, Lightning, Resistance }
+10(10-15)% to Lightning Resistance
{ Unique Modifier — Mana }
+40(30-50) to maximum Mana
{ Unique Modifier }
You have no Accuracy Penalty at Distance — Unscalable Value
{ Unique Modifier — Life }
+44(30-50) to maximum Life
--------
Let the rider's aim be true.
--------
Corrupted");

        Assert.Equal(ItemClass.Helmet, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Ironride", actual.Definition.TradeItem?.Name);
        Assert.Equal("Visored Helm", actual.Definition.TradeItem?.Type);

        Assert.Equal(81, actual.Properties.ItemLevel);
        Assert.Equal(105, actual.Properties.Armour);
        Assert.Equal(89, actual.Properties.EvasionRating);

        Assert.Equal(16, actual.Properties.RequiresLevel);
        Assert.Equal(14, actual.Properties.RequiresStrength);
        Assert.Equal(14, actual.Properties.RequiresDexterity);

        fixture.AssertHasStat(actual, StatCategory.Enchant, "#% increased Mana Regeneration Rate", 30);
    }
}
