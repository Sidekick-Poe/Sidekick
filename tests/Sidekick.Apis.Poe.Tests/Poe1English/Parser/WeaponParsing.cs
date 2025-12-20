using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class WeaponParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseUnidentifiedUnique()
    {
        var actual = parser.ParseItem(@"Item Class: Unknown
Rarity: Unique
Jade Hatchet
--------
One Handed Axe
Physical Damage: 10-15
Critical Strike Chance: 5.00%
Attacks per Second: 1.45
Weapon Range: 11
--------
Requirements:
Str: 21
--------
Sockets: R-G-B
--------
Item Level: 71
--------
Unidentified
;");

        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Jade Hatchet", actual.ApiInformation.Type);
        Assert.True(actual.Properties.Unidentified);
    }

    [Fact]
    public void ParseInfluencedWeapon()
    {
        var actual = parser.ParseItem(@"Item Class: Unknown
Rarity: Rare
Miracle Chant
Imbued Wand
--------
Wand
Physical Damage: 38-69 (augmented)
Critical Strike Chance: 7.00%
Attacks per Second: 1.50
--------
Requirements:
Level: 59
Int: 188
--------
Sockets: R B
--------
Item Level: 70
--------
33% increased Spell Damage (implicit)
--------
Adds 10 to 16 Physical Damage
24% increased Fire Damage
Attacks with this Weapon Penetrate 10% Lightning Resistance
--------
Crusader Item
");

        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Imbued Wand", actual.ApiInformation.Type);
        Assert.Equal("Miracle Chant", actual.Name);
        Assert.True(actual.Properties.Influences.Crusader);

        actual.AssertHasStat(StatCategory.Implicit, "#% increased Spell Damage", 33);
        actual.AssertHasStat(StatCategory.Explicit, "Adds # to # Physical Damage (Local)", 10, 16);
        actual.AssertHasStat(StatCategory.Explicit, "#% increased Fire Damage", 24);
        actual.AssertHasStat(StatCategory.Explicit, "Attacks with this Weapon Penetrate #% Lightning Resistance", 10);
    }

    [Fact]
    public void ParseMagicWeapon()
    {
        var actual = parser.ParseItem(@"Item Class: Unknown
Rarity: Magic
Shadow Axe of the Boxer
--------
Two Handed Axe
Physical Damage: 42-62
Critical Strike Chance: 5.00%
Attacks per Second: 1.25
Weapon Range: 13
--------
Requirements:
Level: 33
Str: 80
Dex: 37
--------
Sockets: R-R
--------
Item Level: 50
--------
11% reduced Enemy Stun Threshold
");

        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Shadow Axe", actual.ApiInformation.Type);

        actual.AssertHasStat(StatCategory.Explicit, "#% reduced Enemy Stun Threshold", 11);
        Assert.False(actual.Stats[0].MatchedFuzzily);
    }

    /// <summary>
    /// This unique item can have multiple possible bases.
    /// </summary>
    [Fact]
    public void ParseUniqueItemWithDifferentBases()
    {
        var actual = parser.ParseItem(@"Item Class: Unknown
Rarity: Unique
Wings of Entropy
Ezomyte Axe
--------
Two Handed Axe
Physical Damage: 144-217 (augmented)
Fire Damage: 81-175 (augmented)
Chaos Damage: 85-177 (augmented)
Critical Strike Chance: 5.70%
Attacks per Second: 1.35
Weapon Range: 13
--------
Requirements:
Level: 62
Str: 140 (unmet)
Dex: 86
--------
Sockets: R-B-R
--------
Item Level: 70
--------
7% Chance to Block Spell Damage
+11% Chance to Block Attack Damage while Dual Wielding
66% increased Physical Damage
Adds 81 to 175 Fire Damage in Main Hand
Adds 85 to 177 Chaos Damage in Off Hand
Counts as Dual Wielding
--------
Fire and Anarchy are the most reliable agents of change.");

        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Wings of Entropy", actual.ApiInformation.Name);
        Assert.Equal("Ezomyte Axe", actual.ApiInformation.Type);

        Assert.Equal(243.7, actual.Properties.PhysicalDps);
        Assert.Equal(172.80, actual.Properties.ElementalDps);
        Assert.Equal(176.9, actual.Properties.ChaosDps);
        Assert.Equal(593.4, actual.Properties.TotalDps);
    }

    [Fact]
    public void ParseDaressoPassion()
    {
        var actual = parser.ParseItem(@"Item Class: Thrusting One Hand Swords
Rarity: Unique
Daresso's Passion
Estoc
--------
One Handed Sword
Physical Damage: 58-90 (augmented)
Elemental Damage: 36-43 (cold)
Critical Strike Chance: 5.50%
Attacks per Second: 1.50
Weapon Range: 1.4 metres
--------
Requirements:
Level: 43
Dex: 140 (unmet)
--------
Sockets: B-G 
--------
Item Level: 84
--------
+25% to Global Critical Strike Multiplier (implicit)
--------
Adds 37 to 40 Physical Damage
Adds 36 to 43 Cold Damage
20% reduced Frenzy Charge Duration
25% chance to gain a Frenzy Charge on Kill
76% increased Damage while you have no Frenzy Charges
--------
It doesn't matter how well the young swordsman trains.
All form and finesse are forgotten when blood first hits the ground.
");

        Assert.Equal(ItemClass.OneHandSword, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Daresso's Passion", actual.ApiInformation.Name);
        Assert.Equal("Estoc", actual.ApiInformation.Type);

        // Verify physical damage
        Assert.Equal(58, actual.Properties.PhysicalDamage?.Min);
        Assert.Equal(90, actual.Properties.PhysicalDamage?.Max);
        Assert.Equal(111, actual.Properties.PhysicalDps);

        // Verify elemental damages
        Assert.Equal(36, actual.Properties.ColdDamage?.Min);
        Assert.Equal(43, actual.Properties.ColdDamage?.Max);

        AssertExtensions.AssertCloseEnough(59.2, actual.Properties.ElementalDps);

        actual.AssertHasStat(StatCategory.Explicit, "#% increased Frenzy Charge Duration", -20);
        Assert.False(actual.Stats[3].MatchedFuzzily);
    }

    [Fact]
    public void Reefbane()
    {
        var actual = parser.ParseItem(@"Item Class: Fishing Rods
Rarity: Unique
Reefbane
Fishing Rod
--------
Fishing Rod
Physical Damage: 8-15
Critical Strike Chance: 5.00%
Attacks per Second: 1.20
Weapon Range: 13
--------
Sockets: G-R R R
--------
Item Level: 85
--------
31% increased Cast Speed
Thaumaturgical Lure
10% increased Quantity of Fish Caught
Glows while in an Area containing a Unique Fish
--------
He cast far into the ocean
And tore out her heart.
--------
Note: ~price 40 chaos
");

        Assert.Equal(ItemClass.FishingRod, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Reefbane", actual.ApiInformation.Name);
        Assert.Equal("Fishing Rod", actual.ApiInformation.Type);
    }

    [Fact]
    public void UnidentifiedHunterItem()
    {
        var actual = parser.ParseItem(@"Item Class: One Hand Maces
Rarity: Rare
Ornate Mace
--------
One Handed Mace
Physical Damage: 53-67
Critical Strike Chance: 5.00%
Attacks per Second: 1.20
Weapon Range: 11
--------
Requirements:
Strength: 161
--------
Sockets: R-R-R
--------
Item Level: 76
--------
15% reduced Enemy Stun Threshold (implicit)
--------
Unidentified
--------
Hunter Item");

        Assert.Equal(ItemClass.OneHandMace, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Ornate Mace", actual.ApiInformation.Type);
        Assert.True(actual.Properties.Influences.Hunter);
    }

    [Fact]
    public void TriggerWeapon()
    {
        var actual = parser.ParseItem(@"Item Class: Thrusting One Hand Swords
Rarity: Rare
Carrion Stinger
Apex Rapier
--------
One Handed Sword
Quality: +20% (augmented)
Physical Damage: 107-236 (augmented)
Elemental Damage: 37-74 (augmented)
Critical Strike Chance: 5.70%
Attacks per Second: 1.40
Weapon Range: 1.4 metres
--------
Requirements:
Level: 60
Dex: 176
--------
Sockets: B-G-B 
--------
Item Level: 75
--------
+35% to Global Critical Strike Multiplier (implicit)
--------
129% increased Physical Damage
Adds 10 to 19 Physical Damage
Adds 37 to 74 Fire Damage
+20% to Lightning Resistance
25% increased Stun Duration on Enemies
22% chance to Impale Enemies on Hit with Attacks
Trigger a Socketed Spell when you Use a Skill, with a 8 second Cooldown (crafted)
Spells Triggered this way have 150% more Cost (crafted)
");

        Assert.Equal(ItemClass.OneHandSword, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Apex Rapier", actual.ApiInformation.Type);

        actual.AssertHasStat(StatCategory.Crafted, "#% chance to Trigger a Socketed Spell on Using a Skill, with a 8 second Cooldown\nSpells Triggered this way have 150% more Cost", 8);
    }

    [Fact]
    public void PhysEleWeapon()
    {
        var actual = parser.ParseItem(@"Item Class: Thrusting One Hand Swords
Rarity: Rare
Phoenix Bane
Fancy Foil
--------
One Handed Sword
Physical Damage: 37-71 (augmented)
Elemental Damage: 56-105 (augmented), 10-175 (augmented)
Critical Strike Chance: 7.04% (augmented)
Attacks per Second: 1.71 (augmented)
Weapon Range: 1.4 metres
--------
Requirements:
Level: 58
Dex: 167
--------
Sockets: G-G G 
--------
Item Level: 57
--------
+25% to Global Critical Strike Multiplier (implicit)
--------
Adds 9 to 20 Physical Damage
Adds 56 to 105 Fire Damage
Adds 10 to 175 Lightning Damage
7% increased Attack Speed
28% increased Critical Strike Chance
9% increased Lightning Damage (crafted)");

        Assert.Equal(ItemClass.OneHandSword, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Fancy Foil", actual.ApiInformation.Type);

        Assert.Equal(110.30, actual.Properties.PhysicalDpsWithQuality);
        Assert.Equal(295.90, actual.Properties.ElementalDps);

        actual.AssertHasStat(StatCategory.Crafted, "#% increased Lightning Damage", 9);
    }

    [Fact]
    public void Staff()
    {
        var actual = parser.ParseItem(@"Item Class: Staves
Rarity: Rare
Armageddon Spire
Imperial Staff
--------
Staff
Physical Damage: 57-171
Critical Strike Chance: 8.50%
Attacks per Second: 1.15
Weapon Range: 1.3 metres
--------
Requirements:
Level: 66
Str: 113
Int: 113
--------
Sockets: G 
--------
Item Level: 83
--------
+25% Chance to Block Spell Damage while wielding a Staff (implicit)
--------
+44% to Damage over Time Multiplier (fractured)
+35% to Chaos Damage over Time Multiplier
25% increased Fire Damage
Adds 25 to 48 Cold Damage to Spells
--------
Fractured Item
--------
Note: ~price 30 chaos
");

        Assert.Equal(ItemClass.Staff, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Imperial Staff", actual.ApiInformation.Type);

        actual.AssertHasStat(StatCategory.Fractured, "+#% to Damage over Time Multiplier", 44);
        actual.AssertHasStat(StatCategory.Explicit, "+#% to Damage over Time Multiplier", 44);

    }

    [Fact]
    public void FortifyMod()
    {
        var actual = parser.ParseItem(@"Item Class: One Hand Axes
Rarity: Unique
Relentless Fury
Decorative Axe
--------
One Handed Axe
Physical Damage: 62-113 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.20
Weapon Range: 1.1 metres
--------
Requirements:
Level: 29
Str: 80
Dex: 23
--------
Sockets: R G R 
--------
Item Level: 83
--------
Adds 4 to 6 Physical Damage (implicit)
Melee Hits have 12% chance to Fortify (implicit)
--------
71% increased Physical Damage
Adds 5 to 10 Physical Damage
2% of Physical Attack Damage Leeched as Life
Culling Strike
You gain Onslaught for 3 seconds on Culling Strike
100% chance to Avoid being Chilled during Onslaught
--------
Relentless fury
Sunder my every foe
Fuel my boiling blood
--------
Corrupted");

        Assert.Equal(ItemClass.OneHandAxe, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Decorative Axe", actual.ApiInformation.Type);

        // Known parsing issue: #912
        actual.AssertDoesNotHaveModifier(StatCategory.Implicit, "Melee Hits have #% chance to Fortify");
        actual.AssertDoesNotHaveModifier(StatCategory.Implicit, "Melee Hits Fortify");
        Assert.Equal(1, actual.Stats.Count(x => x.ApiInformation.FirstOrDefault()?.Category == StatCategory.Implicit));
    }

    [Fact]
    public void PerXModifiers()
    {
        var actual = parser.ParseItem(@"Item Class: Wands
Rarity: Rare
Damnation Call
Prophecy Wand
--------
Wand
Quality: +20% (augmented)
Physical Damage: 31-58 (augmented)
Critical Strike Chance: 8.70%
Attacks per Second: 1.50
--------
Requirements:
Level: 68
Int: 245
--------
Sockets: B-B G 
--------
Item Level: 85
--------
39% increased Spell Damage (implicit)
--------
1% increased Spell Damage per 16 Intelligence
Adds 1 to 5 Lightning Damage to Attacks with this Weapon per 10 Intelligence
Adds 43 to 86 Cold Damage to Spells
29% increased Lightning Damage
10% increased Attack Speed while a Rare or Unique Enemy is Nearby (crafted)
--------
Shaper Item
Elder Item");

        Assert.Equal(ItemClass.Wand, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Prophecy Wand", actual.ApiInformation.Type);

        actual.AssertHasStat(StatCategory.Explicit, "Adds # to # Lightning Damage to Attacks with this Weapon per 10 Intelligence", 1, 5);
        actual.AssertHasStat(StatCategory.Explicit, "#% increased Spell Damage per 16 Intelligence", 1);
    }
}
