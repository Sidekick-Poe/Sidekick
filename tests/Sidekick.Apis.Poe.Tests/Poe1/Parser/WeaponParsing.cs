using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser
{
    [Collection(Collections.Poe1Parser)]
    public class WeaponParsing
    {
        private readonly IItemParser parser;

        public WeaponParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

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
Elemental Damage: 81-175 (augmented)
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

            Assert.Equal(243.68, actual.Properties.PhysicalDps);
            Assert.Equal(172.80, actual.Properties.ElementalDps);
            Assert.Equal(416.48, actual.Properties.DamagePerSecond);
        }

        [Fact]
        public void ParseWeaponWithMultipleElementalDamages()
        {
            var actual = parser.ParseItem(@"Item Class: Unknown
Rarity: Rare
Honour Beak
Ancient Sword
--------
One Handed Sword
Quality: +20% (augmented)
Physical Damage: 22-40 (augmented)
Elemental Damage: 26-48 (augmented), 47-81 (augmented), 4-155 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.74 (augmented)
Weapon Range: 11
--------
Requirements:
Level: 50
Str: 44
Dex: 44
--------
Sockets: R-R B
--------
Item Level: 68
--------
Attribute Modifiers have 8% increased Effect (enchant)
--------
+165 to Accuracy Rating (implicit)
--------
+37 to Dexterity
Adds 26 to 48 Fire Damage
Adds 47 to 81 Cold Damage
Adds 4 to 155 Lightning Damage
20% increased Attack Speed
+21% to Global Critical Strike Multiplier");

            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Ancient Sword", actual.Metadata.Type);

            Assert.Equal(53.94, actual.Properties.PhysicalDps);
            Assert.Equal(314.07, actual.Properties.ElementalDps);
            Assert.Equal(368.01, actual.Properties.DamagePerSecond);
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

            Assert.Equal(Class.FishingRods, actual.Header.Class);
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

            Assert.Equal(Class.OneHandMaces, actual.Header.Class);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal("Ornate Mace", actual.Metadata.Type);
            Assert.True(actual.Influences.Hunter);
        }
    }
}
