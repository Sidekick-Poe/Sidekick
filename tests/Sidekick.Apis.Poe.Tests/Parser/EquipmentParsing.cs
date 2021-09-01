using System.Linq;
using Sidekick.Apis.Poe;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class EquipmentParsing
    {
        private readonly IItemParser parser;

        public EquipmentParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseUnidentifiedUnique()
        {
            var actual = parser.ParseItem(UnidentifiedUnique);

            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal("Jade Hatchet", actual.Metadata.Type);
            Assert.False(actual.Properties.Identified);
        }

        [Fact]
        public void ParseSixLinkUniqueBodyArmor()
        {
            var actual = parser.ParseItem(UniqueSixLink);

            Assert.Equal(Category.Armour, actual.Metadata.Category);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal("Carcass Jack", actual.Metadata.Name);
            Assert.Equal("Varnished Coat", actual.Metadata.Type);
            Assert.Equal(20, actual.Properties.Quality);
            Assert.Equal(960, actual.Properties.Evasion);
            Assert.Equal(186, actual.Properties.EnergyShield);
            Assert.Equal(6, actual.Sockets.Count(x => x.Group == 0));

            var explicits = actual.Modifiers.Explicit.Select(x => x.Text);
            Assert.Contains("128% increased Evasion and Energy Shield (Local)", explicits);
            Assert.Contains("+55 to maximum Life", explicits);
            Assert.Contains("+12% to all Elemental Resistances", explicits);
            Assert.Contains("44% increased Area of Effect", explicits);
            Assert.Contains("47% increased Area Damage", explicits);
            Assert.Contains("Extra gore", explicits);

            var pseudos = actual.Modifiers.Pseudo.Select(x => x.Text);
            Assert.Contains("+12% total to all Elemental Resistances", pseudos);
            Assert.Contains("+36% total Elemental Resistance", pseudos);
            Assert.Contains("+36% total Resistance", pseudos);
            Assert.Contains("+55 total maximum Life", pseudos);

        }

        [Fact]
        public void ParseRareGloves()
        {
            var actual = parser.ParseItem(GlovesAssasinsMitts);

            Assert.Equal(Category.Armour, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Assassin's Mitts", actual.Metadata.Type);
            Assert.Equal("Death Nails", actual.Original.Name);
            Assert.Single(actual.Sockets);

            var explicits = actual.Modifiers.Explicit.Select(x => x.Text);
            Assert.Contains("+18 to Intelligence", explicits);
            Assert.Contains("+73 to maximum Life", explicits);
            Assert.Contains("+14% to Lightning Resistance", explicits);
            Assert.Contains("0.23% of Physical Attack Damage Leeched as Mana", explicits);
        }

        [Fact]
        public void ParseInfluencedWeapon()
        {
            var actual = parser.ParseItem(InfluencedWand);

            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Imbued Wand", actual.Metadata.Type);
            Assert.Equal("Miracle Chant", actual.Original.Name);
            Assert.True(actual.Influences.Crusader);

            var implicits = actual.Modifiers.Implicit.Select(x => x.Text);
            Assert.Contains("33% increased Spell Damage", implicits);

            var explicits = actual.Modifiers.Explicit.Select(x => x.Text);
            Assert.Contains("Adds 10 to 16 Physical Damage", explicits);
            Assert.Contains("24% increased Fire Damage", explicits);
            Assert.Contains("14% increased Critical Strike Chance for Spells", explicits);
            Assert.Contains("Attacks with this Weapon Penetrate 10% Lightning Resistance", explicits);
        }

        [Fact]
        public void ParseMagicWeapon()
        {
            var actual = parser.ParseItem(MagicWeapon);

            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal(Rarity.Magic, actual.Metadata.Rarity);
            Assert.Equal("Shadow Axe", actual.Metadata.Type);

            var explicits = actual.Modifiers.Explicit.Select(x => x.Text);
            Assert.Contains("11% reduced Enemy Stun Threshold", explicits);
        }

        [Fact]
        public void ParseFracturedItem()
        {
            var actual = parser.ParseItem(FracturedItem);

            Assert.Equal(Category.Armour, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Iron Greaves", actual.Metadata.Type);

            var fractureds = actual.Modifiers.Fractured.Select(x => x.Text);
            Assert.Contains("10% increased Movement Speed", fractureds);
        }

        /// <summary>
        /// This unique item can have multiple possible bases.
        /// </summary>
        [Fact]
        public void ParseUniqueItemWithDifferentBases()
        {
            var actual = parser.ParseItem(UniqueItemWithDifferentBases);

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
            var actual = parser.ParseItem(WeaponWithMultipleElementalDamages);

            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Ancient Sword", actual.Metadata.Type);

            Assert.Equal(53.94, actual.Properties.PhysicalDps);
            Assert.Equal(314.07, actual.Properties.ElementalDps);
            Assert.Equal(368.01, actual.Properties.DamagePerSecond);
        }

        [Fact]
        public void ParseEnchantWithAdditionalProjectiles()
        {
            var actual = parser.ParseItem(EnchantWithAdditionalProjectiles);

            var enchants = actual.Modifiers.Enchant.Select(x => x.Text);
            Assert.Contains("Split Arrow fires 2 additional Projectiles", enchants);
            Assert.Equal(2, actual.Modifiers.Enchant.First().Values.First());
        }

        #region ItemText

        private const string UniqueSixLink = @"Item Class: Unknown
Rarity: Unique
Carcass Jack
Varnished Coat
--------
Quality: +20% (augmented)
Evasion Rating: 960 (augmented)
Energy Shield: 186 (augmented)
--------
Requirements:
Level: 70
Str: 68
Dex: 96
Int: 111
--------
Sockets: B-B-R-B-B-B 
--------
Item Level: 81
--------
128% increased Evasion and Energy Shield
+55 to maximum Life
+12% to all Elemental Resistances
44% increased Area of Effect
47% increased Area Damage
Extra gore
--------
""...The discomfort shown by the others is amusing, but none
can deny that my work has made quite the splash...""
- Maligaro's Journal
";

        private const string UnidentifiedUnique = @"Item Class: Unknown
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
;";

        private const string GlovesAssasinsMitts = @"Item Class: Unknown
Rarity: Rare
Death Nails
Assassin's Mitts
--------
Evasion Rating: 104
Energy Shield: 20
--------
Requirements:
Level: 58
Dex: 45
Int: 45
--------
Sockets: G 
--------
Item Level: 61
--------
+18 to Intelligence
+73 to maximum Life
+14% to Lightning Resistance
0.23% of Physical Attack Damage Leeched as Mana
";

        private const string InfluencedWand = @"Item Class: Unknown
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
14% increased Critical Strike Chance for Spells
Attacks with this Weapon Penetrate 10% Lightning Resistance
--------
Crusader Item
";

        private const string MagicWeapon = @"Item Class: Unknown
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
";

        private const string FracturedItem = @"Item Class: Unknown
Rarity: Rare
Invasion Track
Iron Greaves
--------
Armour: 6
--------
Sockets: B B 
--------
Item Level: 2
--------
10% increased Movement Speed (fractured)
+5 to maximum Life
Regenerate 1.9 Life per second
+8% to Cold Resistance
--------
Fractured Item
";

        private const string UniqueItemWithDifferentBases = @"Item Class: Unknown
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
Fire and Anarchy are the most reliable agents of change.";

        private const string WeaponWithMultipleElementalDamages = @"Item Class: Unknown
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
+21% to Global Critical Strike Multiplier";

        private const string EnchantWithAdditionalProjectiles = @"Item Class: Unknown
Rarity: Rare
Doom Glance
Hubris Circlet
--------
Energy Shield: 111 (augmented)
--------
Requirements:
Level: 69
Int: 154
--------
Sockets: B-B 
--------
Item Level: 69
--------
Split Arrow fires 2 additional Projectiles (enchant)
--------
+26 to Intelligence
+4 to maximum Energy Shield
39% increased Energy Shield
+25 to maximum Life";

        #endregion
    }
}
