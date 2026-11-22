using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Stats;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe2.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class BootParsing(Poe2EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void Thunderstep()
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
        Assert.Equal("Steeltoe Boots", actual.TradeItem?.Type);
        Assert.Equal("Thunderstep", actual.TradeItem?.Name);
        Assert.Equal("Thunderstep Steeltoe Boots", actual.TradeItem?.Text);

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
    public void VictoryHoof()
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
        Assert.Equal("Bastion Sabatons", actual.TradeItem?.Type);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal(59, actual.Properties.RequiresLevel);
        Assert.Equal(44, actual.Properties.RequiresStrength);
        Assert.Equal(44, actual.Properties.RequiresDexterity);

        Assert.Equal(66, actual.Properties.ItemLevel);

        fixture.AssertHasStat(actual, StatCategory.Desecrated, "# to Armour", 32);
    }

    [Fact]
    public void AtzirisStep()
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
        Assert.Equal("Atziri's Step", actual.TradeItem?.Name);
        Assert.Equal("Cinched Boots", actual.TradeItem?.Type);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "Gain Deflection Rating equal to #% of Evasion Rating", 60);
        // Issue #985 fixture.AssertHasStat(actual, StatCategory.Explicit, "#% to amount of Damage Prevented by Deflection", -9);
    }

    [Fact]
    public void LoathTread()
    {
        var actual = parser.ParseItem(@"Item Class: Boots
Rarity: Rare
Loath Tread
Runeforged Sekhema Sandals
--------
Quality: +20% (augmented)
Energy Shield: 203 (augmented)
Runic Ward: 221 (augmented)
--------
Requires: Level 80, 108 Int
--------
Sockets: S 
--------
Item Level: 82
--------
18% increased Armour, Evasion and Energy Shield (rune)
--------
{ Prefix Modifier ""Cheetah's"" (Tier: 2) — Speed }
30% increased Movement Speed
{ Prefix Modifier ""Indomitable"" (Tier: 2) — Energy Shield }
91(80-91)% increased Energy Shield
{ Desecrated Prefix Modifier ""Pulsing"" (Tier: 2) — Energy Shield }
+47(42-47) to maximum Energy Shield
{ Suffix Modifier ""of Tzteosh"" (Tier: 1) — Elemental, Fire, Resistance }
+42(41-45)% to Fire Resistance
{ Suffix Modifier ""of the Thunderhead"" (Tier: 5) — Elemental, Lightning, Resistance }
+21(21-25)% to Lightning Resistance
{ Crafted Suffix Modifier ""of the Stars"" }
+11(10-15) to Spirit
");

        Assert.Equal(ItemClass.Boots, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Runeforged Sekhema Sandals", actual.TradeItem?.Type);

        Assert.Equal(82, actual.Properties.ItemLevel);
        Assert.Equal(203, actual.Properties.EnergyShield);
        Assert.Equal(221, actual.Properties.RunicWard);

        Assert.Equal(80, actual.Properties.RequiresLevel);
        Assert.Equal(108, actual.Properties.RequiresIntelligence);
    }

    [Fact]
    public void TheKnightErrantRunemasteredBase()
    {
        var actual = parser.ParseItem(@"Item Class: Boots
Rarity: Unique
The Knight-errant
Runemastered Mail Sabatons
--------
Armour: 71 (augmented)
Evasion Rating: 62 (augmented)
Runic Ward: 54
--------
Requires: Level 38, 29 Str, 29 (unmet) Dex
--------
Item Level: 39
--------
{ Implicit Modifier — Speed }
5% increased Movement Speed
--------
{ Unique Modifier — Armour, Evasion }
41(30-50)% increased Armour and Evasion
{ Unique Modifier }
+38(30-50) to Stun Threshold
{ Unique Modifier — Speed }
10% increased Movement Speed
{ Unique Modifier — Armour, Evasion }
Iron Reflexes — Unscalable Value
{ Unique Modifier }
+50(30-50) to Ailment Threshold
--------
Some search forever for their path.
");

        Assert.Equal(ItemClass.Boots, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("The Knight-errant", actual.TradeItem?.Name);
        Assert.Equal("Runemastered Mail Sabatons", actual.TradeItem?.Type);

        Assert.Equal(39, actual.Properties.ItemLevel);
    }

    [Fact]
    public void TheKnightErrant()
    {
        var actual = parser.ParseItem(@"Item Class: Boots
Rarity: Unique
The Knight-errant
Mail Sabatons
--------
Armour: 71 (augmented)
Evasion Rating: 62 (augmented)
Runic Ward: 54
--------
Requires: Level 38, 29 Str, 29 (unmet) Dex
--------
Item Level: 39
--------
Some search forever for their path.
");

        Assert.Equal(ItemClass.Boots, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("The Knight-errant", actual.TradeItem?.Name);
        Assert.Equal("Mail Sabatons", actual.TradeItem?.Type);

        Assert.Equal(39, actual.Properties.ItemLevel);
    }
}
