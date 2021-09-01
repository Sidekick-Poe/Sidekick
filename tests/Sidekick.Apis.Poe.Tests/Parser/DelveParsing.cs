using Sidekick.Apis.Poe;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class DelveParsing
    {
        private readonly IItemParser parser;

        public DelveParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParsePotentChaoticResonator()
        {
            var actual = parser.ParseItem(@"Item Class: Unknown
Rarity: Currency
Potent Chaotic Resonator
--------
Stack Size: 1/10
Requires 2 Socketed Fossils
--------
Sockets: D D 
--------
Reforges a rare item with new random modifiers
--------
All sockets must be filled with Fossils before this item can be used.
");

            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal("Potent Chaotic Resonator", actual.Metadata.Type);
        }
    }
}
