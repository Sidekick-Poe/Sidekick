using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser
{
    [Collection(Collections.Poe1Parser)]
    public class BreachParsing
    {
        private readonly IItemParser parser;

        public BreachParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void SplinterOfTul()
        {
            var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Splinter of Tul
--------
Stack Size: 9/100
--------
Combine 100 Splinters to create Tul's Breachstone.
Shift click to unstack.
");

            Assert.Equal(Class.StackableCurrency, actual.Header.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Splinter of Tul", actual.Metadata.Type);
        }
    }
}
