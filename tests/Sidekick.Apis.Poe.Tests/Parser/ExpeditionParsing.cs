using System.Linq;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class ExpeditionParsing
    {
        private readonly IItemParser parser;

        public ExpeditionParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseMagicLogbook()
        {
            var actual = parser.ParseItem(@"Item Class: Expedition Logbooks
Rarity: Magic
Chaining Expedition Logbook
--------
Item Quantity: +16% (augmented)
Item Rarity: +9% (augmented)
Monster Pack Size: +6% (augmented)
Area Level: 69
--------
Item Level: 69
--------
Rotting Temple
Druids of the Broken Circle
32% increased quantity of Artifacts dropped by Monsters (implicit)
35% increased Explosive Radius (implicit)
Area contains an additional Underground Area (implicit)
--------
Sarn Slums
Druids of the Broken Circle
29% increased Explosive Radius (implicit)
Area contains 17% increased number of Monster Markers (implicit)
Area contains 36% increased number of Remnants (implicit)
--------
Scrublands
Order of the Chalice
Area contains 8 additional Chest Markers (implicit)
31% increased Explosive Placement Range (implicit)
Area contains 30% increased number of Monster Markers (implicit)
--------
Monsters' skills Chain 2 additional times
--------
Take this item to Dannig in your Hideout to open portals to an expedition.
");

            Assert.Equal(Class.Logbooks, actual.Metadata.Class);
            Assert.Equal(Rarity.Magic, actual.Metadata.Rarity);
            Assert.Equal(Category.Logbook, actual.Metadata.Category);
            Assert.Equal("Expedition Logbook", actual.Metadata.Type);

            var pseudos = actual.Modifiers.Pseudo.Select(x => x.Text);
            Assert.Contains("Has Logbook Faction: Druids of the Broken Circle", pseudos);
            Assert.Contains("Has Logbook Faction: Order of the Chalice", pseudos);
        }
    }
}
