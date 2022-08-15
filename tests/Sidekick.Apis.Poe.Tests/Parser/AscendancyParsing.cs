using System.Linq;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class AscendancyParsing
    {
        private readonly IItemParser parser;

        public AscendancyParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseEnchantWithAdditionalProjectiles()
        {
            var actual = parser.ParseItem(@"Item Class: Unknown
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
+25 to maximum Life");

            var modifiers = actual.ModifierLines.Select(x => x.Modifier?.Text);
            Assert.Contains("Split Arrow fires 2 additional Projectiles", modifiers);
            Assert.Equal(2, actual.ModifierLines.First().Modifier.Values.First());
        }

        [Fact]
        public void TributeToTheGoddess()
        {
            var actual = parser.ParseItem(@"Item Class: Map Fragments
Rarity: Normal
Tribute to the Goddess
--------
You may appeal to the Goddess for another verdict,
but justice favours only the truly worthy.
--------
Travel to the Aspirants' Plaza and spend this item to open the Eternal Labyrinth of Fortune. You must have completed the six different Trials of Ascendancy found in Maps in order to access this area.
");

            Assert.Equal(Class.MapFragments, actual.Metadata.Class);
            Assert.Equal(Rarity.Normal, actual.Metadata.Rarity);
            Assert.Equal(Category.Map, actual.Metadata.Category);
            Assert.Equal("Tribute to the Goddess", actual.Metadata.Type);
        }
    }
}
