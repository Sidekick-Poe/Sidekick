using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser
{
    [Collection(Collections.Poe1Parser)]
    public class DelveParsing(ParserFixture fixture)
    {
        private readonly IItemParser parser = fixture.Parser;

        [Fact]
        public void ParsePotentChaoticResonator()
        {
            var actual = parser.ParseItem(@"Item Class: Delve Stackable Socketable Currency
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
--------
Note: ~price 1 chaos
");

            Assert.Equal("currency", actual.Header.ItemCategory);
            Assert.Equal(Category.Currency, actual.Header.Category);
            Assert.Equal(Rarity.Currency, actual.Header.Rarity);
            Assert.Equal("Potent Chaotic Resonator", actual.Header.ApiType);
        }

        [Fact]
        public void PowerfulChaoticResonator()
        {
            var actual = parser.ParseItem(@"Item Class: Delve Stackable Socketable Currency
Rarity: Currency
Powerful Chaotic Resonator
--------
Stack Size: 1/10
Requires 3 Socketed Fossils
--------
Sockets: D D D 
--------
Reforges a rare item with new random modifiers
--------
All sockets must be filled with Fossils before this item can be used.
--------
Note: ~price 4 chaos
");

            Assert.Equal("currency", actual.Header.ItemCategory);
            Assert.Equal(Rarity.Currency, actual.Header.Rarity);
            Assert.Equal(Category.Currency, actual.Header.Category);
            Assert.Equal("Powerful Chaotic Resonator", actual.Header.ApiType);
        }

        [Fact]
        public void PerfectFossil()
        {
            var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Opulent Fossil
--------
Stack Size: 1/20
--------
More Drop modifiers
No Tagless modifiers
--------
Place in a Resonator to influence item crafting.
");

            Assert.Equal("currency", actual.Header.ItemCategory);
            Assert.Equal(Rarity.Currency, actual.Header.Rarity);
            Assert.Equal(Category.Currency, actual.Header.Category);
            Assert.Equal("Opulent Fossil", actual.Header.ApiType);
        }
    }
}
