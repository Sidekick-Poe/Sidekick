using System.Linq;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class GloveParsing
    {
        private readonly IItemParser parser;

        public GloveParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseRareGloves()
        {
            var actual = parser.ParseItem(@"Item Class: Unknown
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
");

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
    }
}
