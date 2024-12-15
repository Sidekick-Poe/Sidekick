using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser
{
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

            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal("Jade Hatchet", actual.Metadata.Type);
            Assert.False(actual.Properties.Identified);
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

            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Imbued Wand", actual.Metadata.Type);
            Assert.Equal("Miracle Chant", actual.Header.Name);
            Assert.True(actual.Influences.Crusader);

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

            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal(Rarity.Magic, actual.Metadata.Rarity);
            Assert.Equal("Shadow Axe", actual.Metadata.Type);

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

            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal("Wings of Entropy", actual.Metadata.Name);
            Assert.Equal("Ezomyte Axe", actual.Metadata.Type);

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

            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Ancient Sword", actual.Metadata.Type);

            Assert.Equal(53.9, actual.Properties.PhysicalDps);
            Assert.Equal(314.1, actual.Properties.ElementalDps);
            Assert.Equal(368.0, actual.Properties.TotalDps);
        }

        [Fact]
        public void ParseWeaponWithMultipleElementalDamages()
        {
            var actual = parser.ParseItem(@"Item Class: One Hand Swords
Rarity: Rare
Storm Sever
Gemstone Sword
--------
One Handed Sword
Physical Damage: 39-83
Elemental Damage: 5-87 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.30
Weapon Range: 1.1 metres
--------
Requirements:
Level: 56
Str: 96
Dex: 96
--------
Sockets: G 
--------
Item Level: 62
--------
+400 to Accuracy Rating (implicit)
--------
+1 to Level of Socketed Melee Gems
+24 to Strength
Adds 5 to 87 Lightning Damage
6% reduced Enemy Stun Threshold
11% increased Stun Duration on Enemies
");

            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Ancient Sword", actual.Metadata.Type);

            Assert.Equal(53.9, actual.Properties.PhysicalDps);
            Assert.Equal(314.1, actual.Properties.ElementalDps);
            Assert.Equal(368.0, actual.Properties.TotalDps);
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
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal("Reefbane", actual.Metadata.Name);
            Assert.Equal("Fishing Rod", actual.Metadata.Type);
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
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal("Ornate Mace", actual.Metadata.Type);
            Assert.True(actual.Influences.Hunter);
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

            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal(Rarity.Magic, actual.Metadata.Rarity);
            Assert.Equal("Advanced Cultist Bow", actual.Metadata.Type);
            
            // Verify the chaos damage range is parsed correctly
            Assert.Equal(41, actual.Properties.ChaosDamage.Min);
            Assert.Equal(69, actual.Properties.ChaosDamage.Max);
            
            // Verify DPS calculations
            Assert.Equal(66.0, actual.Properties.ChaosDps);
            Assert.Equal(66.0, actual.Properties.TotalDps);
        }

        [Fact]
        public void ParseCombinedElementalDamage()
        {
            var actual = parser.ParseItem(@"Item Class: Bows
Rarity: Rare
Blood Core
Advanced Forlorn Crossbow
--------
Physical Damage: 23-92
Elemental Damage: 20-31 (augmented), 2-91 (augmented)
Critical Hit Chance: 5.00%
Attacks per Second: 1.60
Reload Time: 0.80
--------
Requirements:
Level: 62
Str: 78 (unmet)
Dex: 78 (unmet)
--------
Item Level: 69
--------
Adds 20 to 31 Fire Damage
Adds 2 to 91 Lightning Damage
Grants 3 Life per Enemy Hit");

            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Blood Core", actual.Metadata.Type);
            Assert.Equal("Blood Core", actual.Header.Name);

            // Verify physical damage
            Assert.Equal(23, actual.Properties.PhysicalDamage.Min);
            Assert.Equal(92, actual.Properties.PhysicalDamage.Max);
            Assert.Equal(92.0, actual.Properties.PhysicalDps);

            // Verify elemental damages
            Assert.Equal(2, actual.Properties.ElementalDamages.Count);
            
            // First elemental damage range
            Assert.Equal(20, actual.Properties.ElementalDamages[0].Min);
            Assert.Equal(31, actual.Properties.ElementalDamages[0].Max);
            
            // Second elemental damage range
            Assert.Equal(2, actual.Properties.ElementalDamages[1].Min);
            Assert.Equal(91, actual.Properties.ElementalDamages[1].Max);

            // Verify DPS calculations
            Assert.Equal(115.2, actual.Properties.ElementalDps); // ((20+31)/2 + (2+91)/2) * 1.60
            Assert.Equal(207.2, actual.Properties.TotalDps); // 92.0 + 115.2
        }

        [Fact]
        public void ParseSeparateElementalDamages()
        {
            var actual = parser.ParseItem(@"Item Class: Unknown
Rarity: Rare
Honour Beak
Ancient Sword
--------
One Handed Sword
Quality: +20% (augmented)
Physical Damage: 22-40 (augmented)
Fire Damage: 26-48 (augmented)
Cold Damage: 47-81 (augmented)
Lightning Damage: 4-155 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.74 (augmented)
Weapon Range: 11
--------
Requirements:
Level: 50
Str: 44
Dex: 44
--------
Item Level: 68
--------
Adds 26 to 48 Fire Damage
Adds 47 to 81 Cold Damage
Adds 4 to 155 Lightning Damage");

            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Ancient Sword", actual.Metadata.Type);

            // Verify physical damage
            Assert.Equal(22, actual.Properties.PhysicalDamage.Min);
            Assert.Equal(40, actual.Properties.PhysicalDamage.Max);
            Assert.Equal(53.9, actual.Properties.PhysicalDps);

            // Verify elemental damages
            Assert.Equal(3, actual.Properties.ElementalDamages.Count);
            
            // Fire damage
            Assert.Equal(26, actual.Properties.ElementalDamages[0].Min);
            Assert.Equal(48, actual.Properties.ElementalDamages[0].Max);
            
            // Cold damage
            Assert.Equal(47, actual.Properties.ElementalDamages[1].Min);
            Assert.Equal(81, actual.Properties.ElementalDamages[1].Max);
            
            // Lightning damage
            Assert.Equal(4, actual.Properties.ElementalDamages[2].Min);
            Assert.Equal(155, actual.Properties.ElementalDamages[2].Max);

            // Verify DPS calculations
            Assert.Equal(314.1, actual.Properties.ElementalDps); // ((26+48)/2 + (47+81)/2 + (4+155)/2) * 1.74
            Assert.Equal(368.0, actual.Properties.TotalDps); // 53.9 + 314.1
        }

        [Fact]
        public void ParseMixedDamageTypes()
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
Str: 140
Dex: 86
--------
Item Level: 70
--------
7% Chance to Block Spell Damage");

            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal("Wings of Entropy", actual.Metadata.Name);

            // Verify physical damage
            Assert.Equal(144, actual.Properties.PhysicalDamage.Min);
            Assert.Equal(217, actual.Properties.PhysicalDamage.Max);
            Assert.Equal(243.7, actual.Properties.PhysicalDps);

            // Verify elemental damage
            Assert.Single(actual.Properties.ElementalDamages);
            Assert.Equal(81, actual.Properties.ElementalDamages[0].Min);
            Assert.Equal(175, actual.Properties.ElementalDamages[0].Max);
            Assert.Equal(172.8, actual.Properties.ElementalDps);

            // Verify chaos damage
            Assert.Equal(85, actual.Properties.ChaosDamage.Min);
            Assert.Equal(177, actual.Properties.ChaosDamage.Max);
            Assert.Equal(176.9, actual.Properties.ChaosDps);

            // Verify total DPS
            Assert.Equal(593.4, actual.Properties.TotalDps); // 243.7 + 172.8 + 176.9
        }
    }
}
