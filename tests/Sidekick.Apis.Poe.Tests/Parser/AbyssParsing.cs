using System.Linq;
using Sidekick.Apis.Poe;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class AbyssParsing
    {
        private readonly IItemParser parser;

        public AbyssParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseBulbonicTrail()
        {
            var actual = parser.ParseItem(@"Item Class: Unknown
Rarity: Unique
Bubonic Trail
Murder Boots
--------
Evasion Rating: 185
Energy Shield: 17
--------
Requirements:
Level: 69
Dex: 82
Int: 42
--------
Sockets: G A 
--------
Item Level: 76
--------
Has 1 Abyssal Socket
Triggers Level 20 Death Walk when Equipped
4% increased maximum Life
30% increased Movement Speed
10% increased Damage for each type of Abyss Jewel affecting you
--------
Even the dead serve the Lightless.
");

            Assert.Equal(Category.Armour, actual.Metadata.Category);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal("Bubonic Trail", actual.Metadata.Name);
            Assert.Equal("Murder Boots", actual.Metadata.Type);

            var explicits = actual.Modifiers.Explicit.Select(x => x.Text);
            Assert.Contains("Has 1 Abyssal Socket", explicits);
        }
    }
}
