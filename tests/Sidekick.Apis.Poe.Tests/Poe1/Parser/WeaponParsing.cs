using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class WeaponParsing(ParserFixture fixture)
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

        Assert.Equal(Category.Weapon, actual.Header.Category);
        Assert.Equal(Rarity.Unique, actual.Header.Rarity);
        Assert.Equal("Jade Hatchet", actual.Header.ApiType);
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

        Assert.Equal(Category.Weapon, actual.Header.Category);
        Assert.Equal(Rarity.Rare, actual.Header.Rarity);
        Assert.Equal("Imbued Wand", actual.Header.ApiType);
        Assert.Equal("Miracle Chant", actual.Header.Name);
        Assert.True(actual.Properties.Influences.Crusader);

        actual.AssertHasModifier(ModifierCategory.Implicit, "#% increased Spell Damage", 33);
        actual.AssertHasModifier(ModifierCategory.Explicit, "Adds # to # Physical Damage (Local)", 10, 16);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Fire Damage", 24);
        actual.AssertHasModifier(ModifierCategory.Explicit, "Attacks with this Weapon Penetrate #% Lightning Resistance", 10);
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

        Assert.Equal(Category.Weapon, actual.Header.Category);
        Assert.Equal(Rarity.Magic, actual.Header.Rarity);
        Assert.Equal("Shadow Axe", actual.Header.ApiType);

        actual.AssertHasModifier(ModifierCategory.Explicit, "#% reduced Enemy Stun Threshold", 11);
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

        Assert.Equal(Category.Weapon, actual.Header.Category);
        Assert.Equal(Rarity.Unique, actual.Header.Rarity);
        Assert.Equal("Wings of Entropy", actual.Header.ApiName);
        Assert.Equal("Ezomyte Axe", actual.Header.ApiType);

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
Elemental Damage: 36-43 (augmented)
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

        Assert.Equal(Category.Weapon, actual.Header.Category);
        Assert.Equal(Rarity.Unique, actual.Header.Rarity);
        Assert.Equal("Daresso's Passion", actual.Header.ApiName);
        Assert.Equal("Estoc", actual.Header.ApiType);

        // Verify physical damage
        Assert.Equal(58, actual.Properties.PhysicalDamage?.Min);
        Assert.Equal(90, actual.Properties.PhysicalDamage?.Max);
        Assert.Equal(111, actual.Properties.PhysicalDps);

        // Verify elemental damages
        Assert.Equal(36, actual.Properties.ColdDamage?.Min);
        Assert.Equal(43, actual.Properties.ColdDamage?.Max);

        AssertHelper.CloseEnough(59.2, actual.Properties.ElementalDps);
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

        Assert.Equal("weapon.rod", actual.Header.ItemCategory);
        Assert.Equal(Rarity.Unique, actual.Header.Rarity);
        Assert.Equal(Category.Weapon, actual.Header.Category);
        Assert.Equal("Reefbane", actual.Header.ApiName);
        Assert.Equal("Fishing Rod", actual.Header.ApiType);
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

        Assert.Equal("weapon.onemace", actual.Header.ItemCategory);
        Assert.Equal(Rarity.Rare, actual.Header.Rarity);
        Assert.Equal(Category.Weapon, actual.Header.Category);
        Assert.Equal("Ornate Mace", actual.Header.ApiType);
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

        Assert.Equal("weapon.onesword", actual.Header.ItemCategory);
        Assert.Equal(Rarity.Rare, actual.Header.Rarity);
        Assert.Equal(Category.Weapon, actual.Header.Category);
        Assert.Equal("Apex Rapier", actual.Header.ApiType);

        actual.AssertHasModifier(ModifierCategory.Crafted, "#% chance to Trigger a Socketed Spell on Using a Skill, with a 8 second Cooldown\nSpells Triggered this way have 150% more Cost", 8, 150);
    }
}
