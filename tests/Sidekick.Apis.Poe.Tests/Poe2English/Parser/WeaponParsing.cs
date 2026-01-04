using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe2English.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class WeaponParsing(Poe2EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseStaff()
    {
        var actual = parser.ParseItem(
            @"Item Class: Staves
Rarity: Magic
Chalybeous Ashen Staff of the Augur
--------
Requirements:
Level: 58
Int: 133 (unmet)
--------
Item Level: 60
--------
+148 to maximum Mana
+20 to Intelligence
");

        Assert.Equal(ItemClass.Staff, actual.Properties.ItemClass);
        Assert.Equal("Ashen Staff", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);
        Assert.Equal(60, actual.Properties.ItemLevel);

        actual.AssertHasStat(StatCategory.Explicit, "# to maximum Mana", 148);
        actual.AssertHasStat(StatCategory.Explicit, "# to Intelligence", 20);
    }

    [Fact]
    public void ParseColdBow()
    {
        var actual = parser.ParseItem(
            @"Item Class: Bows
Rarity: Rare
Brood Fletch
Expert Composite Bow
--------
Quality: +14% (augmented)
Physical Damage: 177-289 (augmented)
Cold Damage: 39-75 (augmented)
Critical Hit Chance: 5.00%
Attacks per Second: 1.20
--------
Requirements:
Level: 72
Dex: 125 (augmented)
--------
Sockets: S S 
--------
Item Level: 76
--------
40% increased Physical Damage (rune)
--------
73% increased Physical Damage
Adds 24 to 37 Physical Damage
Adds 39 to 75 Cold Damage
35% reduced Attribute Requirements
+3 to Level of all Projectile Skills
Leeches 4.02% of Physical Damage as Mana
");

        Assert.Equal(ItemClass.Bow, actual.Properties.ItemClass);
        Assert.Equal("Composite Bow", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);
        Assert.Equal(76, actual.Properties.ItemLevel);

        // Verify physical damage
        Assert.Equal(177, actual.Properties.PhysicalDamage?.Min);
        Assert.Equal(289, actual.Properties.PhysicalDamage?.Max);
        Assert.Equal(279.6, actual.Properties.PhysicalDps);

        // Verify elemental damages
        Assert.Equal(39, actual.Properties.ColdDamage?.Min);
        Assert.Equal(75, actual.Properties.ColdDamage?.Max);
        Assert.Equal(68.4, actual.Properties.ElementalDps);

        actual.AssertHasStat(StatCategory.Rune, "#% increased Physical Damage", 40);
        actual.AssertHasStat(StatCategory.Explicit, "#% increased Physical Damage", 73);
        actual.AssertHasStat(StatCategory.Explicit, "Adds # to # Physical Damage", 24, 37);
        actual.AssertHasStat(StatCategory.Explicit, "Adds # to # Cold Damage", 39, 75);
        actual.AssertHasStat(StatCategory.Explicit, "#% increased Attribute Requirements", -35);
        Assert.False(actual.Stats[4].MatchedFuzzily);
        actual.AssertHasStat(StatCategory.Explicit, "# to Level of all Projectile Skills", 3);
        actual.AssertHasStat(StatCategory.Explicit, "Leeches #% of Physical Damage as Mana", 4.02);
    }

    [Fact]
    public void ParseElementalCrossbow()
    {
      var actual = parser.ParseItem(@"Item Class: Crossbows
Rarity: Rare
Blood Core
Bleak Crossbow
--------
Physical Damage: 23-92
Elemental Damage: 20-31 (fire), 2-91 (lightning)
Critical Hit Chance: 5.00%
Attacks per Second: 1.60
Reload Time: 0.80
--------
Requirements:
Level: 62
Str: 78
Dex: 78
--------
Item Level: 69
--------
Adds 20 to 31 Fire Damage
Adds 2 to 91 Lightning Damage
Grants 3 Life per Enemy Hit
");

      Assert.Equal(ItemClass.Crossbow, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Bleak Crossbow", actual.ApiInformation.Type);
        Assert.Equal("Blood Core", actual.Name);

        // Verify physical damage
        Assert.Equal(23, actual.Properties.PhysicalDamage?.Min);
        Assert.Equal(92, actual.Properties.PhysicalDamage?.Max);
        Assert.Equal(92.0, actual.Properties.PhysicalDps);

        // Verify elemental damages
        Assert.Equal(20, actual.Properties.FireDamage?.Min);
        Assert.Equal(31, actual.Properties.FireDamage?.Max);

        Assert.Equal(2, actual.Properties.LightningDamage?.Min);
        Assert.Equal(91, actual.Properties.LightningDamage?.Max);

        // Verify DPS calculations
        Assert.Equal(115.2, actual.Properties.ElementalDps); // ((20+31)/2 + (2+91)/2) * 1.60
        AssertExtensions.AssertCloseEnough(207.2, actual.Properties.TotalDps); // 92.0 + 115.2
    }

    [Fact]
    public void ParseChaosDamageWeapon()
    {
        var actual = parser.ParseItem(@"Item Class: Unknown
Rarity: Magic
Advanced Cultist Bow of the Parched
--------
Bow
Chaos Damage: 41-69
Critical Strike Chance: 5.00%
Attacks per Second: 1.20
Weapon Range: 11
--------
Requirements:
Level: 59
Dex: 135
--------
Item Level: 60
--------
Leeches 5.82% of Physical Damage as Mana");

        Assert.Equal(ItemClass.Bow, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Cultist Bow", actual.ApiInformation.Type);

        // Verify the chaos damage range is parsed correctly
        Assert.Equal(41, actual.Properties.ChaosDamage?.Min);
        Assert.Equal(69, actual.Properties.ChaosDamage?.Max);

        // Verify DPS calculations
        Assert.Equal(66.0, actual.Properties.ChaosDps);
        Assert.Equal(66.0, actual.Properties.TotalDps);
    }

    [Fact]
    public void ParseSpirit()
    {
        var actual = parser.ParseItem(@"Item Class: Sceptres
Rarity: Magic
Burning Rattling Sceptre
--------
Spirit: 100
--------
Requires: Level 66, 46 Str, 117 Int
--------
Item Level: 70
--------
Allies in your Presence deal 9 to 13 additional Attack Fire Damage
");

        Assert.Equal(ItemClass.Sceptre, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Rattling Sceptre", actual.ApiInformation.Type);
        Assert.Equal(66, actual.Properties.RequiresLevel);
        Assert.Equal(46, actual.Properties.RequiresStrength);
        Assert.Equal(117, actual.Properties.RequiresIntelligence);

        Assert.Equal(100, actual.Properties.Spirit);
    }

    [Fact]
    public void ParseSpear()
    {
        var actual = parser.ParseItem(@"Item Class: Spears
Rarity: Magic
Precise Ironhead Spear
--------
Physical Damage: 7-13
Critical Hit Chance: 5.00%
Attacks per Second: 1.60
--------
Requires: Level 5, 11 Dex
--------
Item Level: 5
--------
Grants Skill: Spear Throw
--------
+32 to Accuracy Rating

");

        Assert.Equal(ItemClass.Spear, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Ironhead Spear", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);

        actual.AssertHasStat(StatCategory.Explicit, "# to Accuracy Rating", 32);
    }

    [Fact]
    public void ParseElementalSpear()
    {
        var actual = parser.ParseItem(@"Item Class: Spears
Rarity: Rare
Hypnotic Edge
Forked Spear
--------
Quality: +11% (augmented)
Physical Damage: 31-52 (augmented)
Elemental Damage: 2-4 (cold), 3-67 (lightning)
Critical Hit Chance: 6.40% (augmented)
Attacks per Second: 1.84 (augmented)
--------
Requires: Level 26, 20 Str, 48 Dex
--------
Sockets: S 
--------
Item Level: 32
--------
Adds 1 to 20 Lightning Damage (rune)
--------
Grants Skill: Spear Throw
--------
Adds 11 to 15 Physical Damage
Adds 2 to 4 Cold Damage
Adds 2 to 47 Lightning Damage
+1.4% to Critical Hit Chance
15% increased Attack Speed
Grants 3 Life per Enemy Hit
");

        Assert.Equal(ItemClass.Spear, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Forked Spear", actual.ApiInformation.Type);
        Assert.Equal("Hypnotic Edge", actual.Name);

        // Verify physical damage
        Assert.Equal(31, actual.Properties.PhysicalDamage?.Min);
        Assert.Equal(52, actual.Properties.PhysicalDamage?.Max);
        AssertExtensions.AssertCloseEnough(76.4, actual.Properties.PhysicalDps);

        // Verify elemental damages
        Assert.Equal(2, actual.Properties.ColdDamage?.Min);
        Assert.Equal(4, actual.Properties.ColdDamage?.Max);

        Assert.Equal(3, actual.Properties.LightningDamage?.Min);
        Assert.Equal(67, actual.Properties.LightningDamage?.Max);

        // Verify DPS calculations
        AssertExtensions.AssertCloseEnough(69.9, actual.Properties.ElementalDps);
        AssertExtensions.AssertCloseEnough(146.3, actual.Properties.TotalDps);
    }

    [Fact]
    public void ParseElementalQuarterstaff()
    {
        var actual = parser.ParseItem(@"Item Class: Quarterstaves
Rarity: Rare
Kraken Pillar
Slicing Quarterstaff
--------
Quality: +20% (augmented)
Physical Damage: 45-94 (augmented)
Elemental Damage: 14-21 (fire), 14-27 (cold), 1-46 (lightning)
Critical Hit Chance: 10.00%
Attacks per Second: 1.65 (augmented)
--------
Requires: Level 33, 60 Dex, 25 Int
--------
Sockets: S S 
--------
Item Level: 35
--------
7% increased Attack Speed (enchant)
--------
30% increased Physical Damage (rune)
--------
Adds 14 to 21 Fire Damage
Adds 14 to 27 Cold Damage
Adds 1 to 46 Lightning Damage
11% increased Attack Speed
+3 to Level of all Melee Skills
Leeches 5.21% of Physical Damage as Life
--------
Corrupted
");

        Assert.Equal(ItemClass.Warstaff, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Slicing Quarterstaff", actual.ApiInformation.Type);
        Assert.Equal("Kraken Pillar", actual.Name);

        // Verify physical damage
        Assert.Equal(45, actual.Properties.PhysicalDamage?.Min);
        Assert.Equal(94, actual.Properties.PhysicalDamage?.Max);
        AssertExtensions.AssertCloseEnough(114.7, actual.Properties.PhysicalDps);

        // Verify elemental damages
        Assert.Equal(14, actual.Properties.FireDamage?.Min);
        Assert.Equal(21, actual.Properties.FireDamage?.Max);

        Assert.Equal(14, actual.Properties.ColdDamage?.Min);
        Assert.Equal(27, actual.Properties.ColdDamage?.Max);

        Assert.Equal(1, actual.Properties.LightningDamage?.Min);
        Assert.Equal(46, actual.Properties.LightningDamage?.Max);

        // Verify DPS calculations
        AssertExtensions.AssertCloseEnough(101.5, actual.Properties.ElementalDps);
        AssertExtensions.AssertCloseEnough(216.2, actual.Properties.TotalDps);
    }

    [Fact]
    public async Task ParseThousandDamage()
    {
        var actual = parser.ParseItem(@"Item Class: Crossbows
Rarity: Rare
Dragon Core
Siege Crossbow
--------
Quality: +29% (augmented)
Physical Damage: 414-1,043 (augmented)
Critical Hit Chance: 5.00%
Attacks per Second: 2.07 (augmented)
Reload Time: 0.60 (augmented)
--------
Requires: Level 79, 89 (unmet) Str, 89 Dex
--------
Sockets: S S 
--------
Item Level: 82
--------
36% increased Physical Damage (rune)
--------
Grenade Skills Fire an additional Projectile (implicit)
--------
25% increased Attack Speed (fractured)
251% increased Physical Damage
+175 to Accuracy Rating
+7 to Level of all Projectile Skills
Loads 2 additional bolts
Adds 54 to 94 Physical Damage (desecrated)
--------
Fractured Item
--------
Note: ~price 1 mirror");

        Assert.Equal(ItemClass.Crossbow, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Siege Crossbow", actual.ApiInformation.Type);
        Assert.Equal("Dragon Core", actual.Name);

        // Verify physical damage
        Assert.Equal(414, actual.Properties.PhysicalDamage?.Min);
        Assert.Equal(1043, actual.Properties.PhysicalDamage?.Max);

        actual.AssertHasStat(StatCategory.Rune, "#% increased Physical Damage", 36);

        actual.AssertHasStat(StatCategory.Fractured, "#% increased Attack Speed", 25);
        actual.AssertHasStat(StatCategory.Explicit, "#% increased Attack Speed", 25);

        actual.AssertHasStat(StatCategory.Desecrated, "Adds # to # Physical Damage", 54, 94);
        actual.AssertHasStat(StatCategory.Explicit, "Adds # to # Physical Damage", 54, 94);

        actual.AssertHasStat(StatCategory.Explicit, "Loads an additional bolt", 2);
        actual.AssertDoesNotHaveModifier(StatCategory.Fractured, "Loads an additional bolt");

        var modifierFilters = await fixture.StatParser.GetFilters(actual);

        var fracturedFilter = modifierFilters
            .OfType<StatFilter>()
            .First(x => x.PrimaryCategory == StatCategory.Fractured);
        Assert.True(fracturedFilter.UsePrimaryCategory);
        Assert.Equal(StatCategory.Fractured, fracturedFilter.PrimaryCategory);
        Assert.Equal(StatCategory.Explicit, fracturedFilter.SecondaryCategory);
        foreach (var x in fracturedFilter.Stat.ApiInformation)
        {
            if (x.Category is StatCategory.Fractured or StatCategory.Explicit) continue;

            Assert.Fail();
        }

        var desecratedFilter = modifierFilters
            .OfType<StatFilter>()
            .First(x => x.PrimaryCategory == StatCategory.Desecrated);
        Assert.False(desecratedFilter.UsePrimaryCategory);
        Assert.Equal(StatCategory.Desecrated, desecratedFilter.PrimaryCategory);
        Assert.Equal(StatCategory.Explicit, desecratedFilter.SecondaryCategory);
        foreach (var x in desecratedFilter.Stat.ApiInformation)
        {
            if (x.Category is StatCategory.Desecrated or StatCategory.Explicit) continue;

            Assert.Fail();
        }
    }

    [Fact]
    public void ParseTalisman()
    {
        var actual = parser.ParseItem(@"Item Class: Talismans
Rarity: Magic
Lumbering Talisman of Consumption
--------
Physical Damage: 71-107
Critical Hit Chance: 5.00%
Attacks per Second: 1.10
--------
Requires: Level 52, 60 Str, 43 Int
--------
Item Level: 77
--------
Gain 19 Mana per enemy killed
");

        Assert.Equal(ItemClass.Talisman, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Lumbering Talisman", actual.ApiInformation.Type);
        Assert.Null(actual.Name);

        Assert.Equal(77, actual.Properties.ItemLevel);

        Assert.Equal(71, actual.Properties.PhysicalDamage?.Min);
        Assert.Equal(107, actual.Properties.PhysicalDamage?.Max);
        Assert.Equal(5, actual.Properties.CriticalHitChance);
        Assert.Equal(1.1, actual.Properties.AttacksPerSecond);

        Assert.Equal(0, actual.Properties.RequiresDexterity);
        Assert.Equal(43, actual.Properties.RequiresIntelligence);
        Assert.Equal(60, actual.Properties.RequiresStrength);
        Assert.Equal(52, actual.Properties.RequiresLevel);
    }
}
