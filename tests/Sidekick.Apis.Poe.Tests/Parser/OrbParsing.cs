using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class OrbParsing
    {
        private readonly IItemParser parser;

        public OrbParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ChaosOrb()
        {
            var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Chaos Orb
--------
Stack Size: 1/10
--------
Reforges a rare item with new random modifiers
--------
Right click this item then left click a rare item to apply it.
--------
Note: ~b/o 2 blessed
");

            Assert.Equal(Class.StackableCurrency, actual.Metadata.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Chaos Orb", actual.Metadata.Type);
        }
    }
}
