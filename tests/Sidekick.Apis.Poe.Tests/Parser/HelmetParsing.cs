using System.Linq;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class HelmetParsing
    {
        private readonly IItemParser parser;

        public HelmetParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseBlightGuardian()
        {
            var actual = parser.ParseItem(BlightGuardian);

            Assert.Equal(Category.Armour, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Hunter Hood", actual.Metadata.Type);

            var modifiers = actual.ModifierLines.Select(x => x.Modifier?.Text);
            Assert.Contains("You have Shocking Conflux for 3 seconds every 8 seconds", modifiers);
        }

        [Fact]
        public void ParseStarkonjaHead()
        {
            var actual = parser.ParseItem(StarkonjaHead);

            Assert.Equal(Category.Armour, actual.Metadata.Category);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal("Starkonja's Head", actual.Metadata.Name);
            Assert.Equal("Silken Hood", actual.Metadata.Type);

            var modifiers = actual.ModifierLines.Select(x => x.Modifier?.Text);
            Assert.Contains("Divine Ire Damages 2 additional nearby Enemies when gaining Stages", modifiers);
        }

        #region ItemText

        private const string BlightGuardian = @"Item Class: Unknown
Rarity: Rare
Blight Guardian
Hunter Hood
--------
Evasion Rating: 231 (augmented)
--------
Requirements:
Level: 64
Dex: 87
--------
Sockets: G
--------
Item Level: 80
--------
Adds 28 to 51 Fire Damage to Spells
+28 to Evasion Rating
+47 to maximum Life
11% increased Rarity of Items found
+29% to Cold Resistance
You have Shocking Conflux for 3 seconds every 8 seconds
--------
Hunter Item
";

        private const string StarkonjaHead = @"Item Class: Unknown
Rarity: Unique
Starkonja's Head
Silken Hood
--------
Evasion Rating: 765 (augmented)
--------
Requirements:
Level: 60
Dex: 138
--------
Sockets: G G
--------
Item Level: 80
--------
Divine Ire Damages 2 additional nearby Enemies when gaining Stages (enchant)
--------
+57 to Dexterity
50% reduced Damage when on Low Life
10% increased Attack Speed
25% increased Global Critical Strike Chance
121% increased Evasion Rating
+87 to maximum Life
150% increased Global Evasion Rating when on Low Life
--------
There was no hero made out of Starkonja's death,
but merely a long sleep made eternal.
";

        #endregion ItemText
    }
}
