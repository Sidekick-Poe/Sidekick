using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class MapParsing
    {
        private readonly IItemParser parser;

        public MapParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseArcadeMap()
        {
            var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Arcade Map
--------
Map Tier: 15
Atlas Region: Haewark Hamlet
--------
Item Level: 84
--------
Travel to this Map by using it in a personal Map Device. Maps can only be used once.
");

            Assert.Equal(Category.Map, actual.Metadata.Category);
            Assert.Equal(Class.Maps, actual.Header.Class);
            Assert.Equal(Rarity.Normal, actual.Metadata.Rarity);
            Assert.Equal("Arcade Map", actual.Metadata.Type);
            Assert.Equal(15, actual.Properties.MapTier);
            Assert.Equal(84, actual.Properties.ItemLevel);
        }

        [Fact]
        public void ParseUniqueMap()
        {
            var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Unique
Maelström of Chaos
Atoll Map
--------
Map Tier: 5
Atlas Region: Tirn's End
Item Quantity: +41% (augmented)
Item Rarity: +299% (augmented)
--------
Item Level: 73
--------
Area has patches of Chilled Ground
Monsters deal 50% extra Physical Damage as Lightning
Monsters are Immune to randomly chosen Elemental Ailments or Stun
Monsters' Melee Attacks apply random Hexes on Hit
Monsters Reflect Hexes
--------
Whispers from a world apart
Speak my name beyond the tomb;
Bound within the Maelström's heart,
Will they grant me strength or doom?
--------
Travel to this Map by using it in a personal Map Device. Maps can only be used once.
");

            Assert.Equal(Category.Map, actual.Metadata.Category);
            Assert.Equal(Class.Maps, actual.Header.Class);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal("Maelström of Chaos", actual.Metadata.Name);
            Assert.Equal("Atoll Map", actual.Metadata.Type);
            Assert.Equal(5, actual.Properties.MapTier);
            Assert.Equal(41, actual.Properties.ItemQuantity);
            Assert.Equal(299, actual.Properties.ItemRarity);
        }

        [Fact]
        public void ParseOccupiedMap()
        {
            var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Vortex Crag
Phantasmagoria Map
--------
Map Tier: 12
Atlas Region: Lex Ejoris
Item Quantity: +120% (augmented)
Item Rarity: +61% (augmented)
Monster Pack Size: +39% (augmented)
Quality: +19% (augmented)
--------
Item Level: 79
--------
Area is influenced by The Elder (implicit)
--------
Players are Cursed with Vulnerability
44% more Monster Life
Monsters reflect 18% of Physical Damage
Area contains two Unique Bosses
Monsters take 39% reduced Extra Damage from Critical Strikes
Monsters gain an Endurance Charge on Hit
Monsters cannot be Taunted
Monsters' Action Speed cannot be modified to below base value
Players gain 50% reduced Flask Charges
--------
Travel to this Map by using it in a personal Map Device. Maps can only be used once.
--------
Corrupted
--------
Note: ~price 2 chaos
");

            Assert.Equal(Category.Map, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Phantasmagoria Map", actual.Metadata.Type);

            actual.AssertHasModifier(ModifierCategory.Implicit, "Area is influenced by The Elder");
        }

        [Fact]
        public void ParseTimelessKaruiEmblem()
        {
            var actual = parser.ParseItem(TimelessKaruiEmblem);

            Assert.Equal(Category.Map, actual.Metadata.Category);
            Assert.Equal(Rarity.Normal, actual.Metadata.Rarity);
            Assert.Equal("Timeless Karui Emblem", actual.Metadata.Type);
        }

        [Fact]
        public void ParseVortexPit()
        {
            var actual = parser.ParseItem(@"Item Class: Unknown
Rarity: Rare
Vortex Pit
Burial Chambers Map
--------
Map Tier: 16
Atlas Region: Lex Ejoris
Item Quantity: +70% (augmented)
Item Rarity: +31% (augmented)
Monster Pack Size: +20% (augmented)
Quality: +18% (augmented)
--------
Item Level: 82
--------
Area is influenced by The Elder (implicit)
Map is occupied by The Eradicator (implicit)
--------
29% more Magic Monsters
22% more Rare Monsters
Monsters have 100% increased Area of Effect
Monsters Poison on Hit
Rare Monsters each have a Nemesis Mod
Magic Monster Packs each have a Bloodline Mod
--------
Travel to this Map by using it in a personal Map Device. Maps can only be used once.
");

            Assert.Equal(Category.Map, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Burial Chambers Map", actual.Metadata.Type);

            actual.AssertHasModifier(ModifierCategory.Explicit, "Monsters have #% increased Area of Effect", 100);
        }

        #region ItemText

        private const string TimelessKaruiEmblem = @"Item Class: Map Fragments
Rarity: Normal
Timeless Karui Emblem
--------
Place two or more different Emblems in a Map Device to access the Domain of Timeless Conflict. Can only be used once.
";

        #endregion ItemText
    }
}
