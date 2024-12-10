using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser
{
    [Collection(Collections.Poe1Parser)]
    public class UltimatumParsing
    {
        private readonly IItemParser parser;

        public UltimatumParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void NoxiousCatalyst()
        {
            var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Noxious Catalyst
--------
Stack Size: 1/10
--------
Adds quality that enhances Physical and Chaos Damage modifiers on a ring, amulet or belt
Replaces other quality types
--------
Right click this item then left click a ring, amulet or belt to apply it. Has greater effect on lower-rarity jewellery. The maximum quality is 20%.
");

            Assert.Equal(Class.StackableCurrency, actual.Header.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Noxious Catalyst", actual.Metadata.Type);
        }
    }
}
