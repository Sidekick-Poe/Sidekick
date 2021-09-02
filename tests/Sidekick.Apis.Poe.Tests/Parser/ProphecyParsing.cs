using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class ProphecyParsing
    {
        private readonly IItemParser parser;

        public ProphecyParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseProphecy()
        {
            var actual = parser.ParseItem(@"Item Class: Unknown
Rarity: Normal
The Four Feral Exiles
--------
In a faraway dream, four souls far from home prepare to fight to the death.
--------
You will enter a map that holds four additional Rogue Exiles.
--------
Right-click to add this prophecy to your character.
");

            Assert.Equal(Category.Prophecy, actual.Metadata.Category);
            Assert.Equal(Rarity.Prophecy, actual.Metadata.Rarity);
            Assert.Equal("The Four Feral Exiles", actual.Metadata.Name);
            Assert.Equal("Prophecy", actual.Metadata.Type);
        }
    }
}
