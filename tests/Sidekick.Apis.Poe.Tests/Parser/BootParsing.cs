using System.Linq;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class BootParsing
    {
        private readonly IItemParser parser;

        public BootParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseFracturedItem()
        {
            var actual = parser.ParseItem(@"Item Class: Unknown
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
");

            Assert.Equal(Category.Armour, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Iron Greaves", actual.Metadata.Type);

            var modifiers = actual.ModifierLines.Select(x => x.Modifier?.Text);
            Assert.Contains("10% increased Movement Speed", modifiers);
        }
    }
}
