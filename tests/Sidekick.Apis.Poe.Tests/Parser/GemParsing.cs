using Sidekick.Apis.Poe;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class GemParsing
    {
        private readonly IItemParser parser;

        public GemParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseVaalGem()
        {
            var actual = parser.ParseItem(VaalGem);

            Assert.Equal(Category.Gem, actual.Metadata.Category);
            Assert.Equal(Rarity.Gem, actual.Metadata.Rarity);
            Assert.Equal("Vaal Double Strike", actual.Metadata.Type);
            Assert.Equal(1, actual.Properties.GemLevel);
            Assert.Equal(0, actual.Properties.Quality);
            Assert.False(actual.Properties.AlternateQuality);
            Assert.True(actual.Properties.Corrupted);
        }

        [Fact]
        public void ParseAnomalousGem()
        {
            var actual = parser.ParseItem(AnomalousGem);

            Assert.Equal(Category.Gem, actual.Metadata.Category);
            Assert.Equal(Rarity.Gem, actual.Metadata.Rarity);
            Assert.Equal("Static Strike", actual.Metadata.Type);
            Assert.Equal(1, actual.Properties.GemLevel);
            Assert.Equal(17, actual.Properties.Quality);
            Assert.True(actual.Properties.AlternateQuality);
            Assert.False(actual.Properties.Corrupted);
        }

        #region ItemText

        private const string VaalGem = @"Item Class: Unknown
Rarity: Gem
Double Strike
--------
Vaal, Attack, Melee, Strike, Duration, Physical
Level: 1
Mana Cost: 5
Attack Speed: 80% of base
Effectiveness of Added Damage: 91%
Experience: 1/70
--------
Requirements:
Level: 1
--------
Performs two fast strikes with a melee weapon.
--------
Deals 91.3% of Base Damage
25% chance to cause Bleeding
Adds 3 to 5 Physical Damage against Bleeding Enemies
--------
Vaal Double Strike
--------
Souls Per Use: 30
Can Store 2 Uses
Soul Gain Prevention: 8 sec
Effectiveness of Added Damage: 28%
--------
Performs two fast strikes with a melee weapon, each of which summons a double of you for a duration to continuously attack monsters in this fashion.
--------
Deals 28% of Base Damage
Base duration is 6.00 seconds
Modifiers to Skill Effect Duration also apply to this Skill's Soul Gain Prevention
Can't be Evaded
25% chance to cause Bleeding
Adds 3 to 5 Physical Damage against Bleeding Enemies
--------
Place into an item socket of the right colour to gain this skill.Right click to remove from a socket.
--------
Corrupted
--------
Note: ~price 2 chaos
";

        private const string AnomalousGem = @"Item Class: Unknown
Rarity: Gem
Anomalous Static Strike
--------
Attack, Melee, Strike, AoE, Duration, Lightning, Chaining
Level: 1
Mana Cost: 6
Effectiveness of Added Damage: 110%
Quality: +17% (augmented)
Alternate Quality
--------
Requirements:
Level: 12
Str: 21
Int: 14
--------
Attack with a melee weapon, gaining static energy for a duration if you hit an enemy. While you have static energy, you'll frequently hit a number of nearby enemies with beams, dealing attack damage.
--------
Beams Hit Enemies every 0.40 seconds
50% of Physical Damage Converted to Lightning Damage
Deals 110% of Base Damage
Base duration is 2.00 seconds
Chains +1 Times
17% increased Damage
Beams deal 40% less Damage
4 maximum Beam Targets
--------
Experience: 1/15249
--------
Place into an item socket of the right colour to gain this skill. Right click to remove from a socket.
";

        #endregion
    }
}
