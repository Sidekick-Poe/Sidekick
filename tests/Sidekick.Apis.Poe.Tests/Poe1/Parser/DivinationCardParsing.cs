using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser
{
    [Collection(Collections.Poe1Parser)]
    public class DivinationCardParsing(ParserFixture fixture)
    {
        private readonly IItemParser parser = fixture.Parser;

        [Fact]
        public void ParseSaintTreasure()
        {
            var actual = parser.ParseItem(@"Item Class: Divination Cards
Rarity: Divination Card
The Saint's Treasure
--------
Stack Size: 1/10
--------
2x Exalted Orb
--------
Publicly, he lived a pious and chaste life of poverty. Privately, tithes and tributes made him and his lascivious company very comfortable indeed.
");

            Assert.Equal("card", actual.Header.ItemCategory);
            Assert.Equal(Category.DivinationCard, actual.Header.Category);
            Assert.Equal(Rarity.DivinationCard, actual.Header.Rarity);
            Assert.Null(actual.Header.ApiName);
            Assert.Equal("The Saint's Treasure", actual.Header.ApiType);
        }

        [Fact]
        public void ParseLordOfCelebration()
        {
            var actual = parser.ParseItem(@"Item Class: Divination Cards
Rarity: Divination Card
The Lord of Celebration
--------
Stack Size: 1/4
--------
Sceptre of Celebration
Shaper Item
--------
Though they were a pack of elite combatants, the Emperor's royal guards were not ready to face one of his notorious parties.");

            Assert.Equal("card", actual.Header.ItemCategory);
            Assert.Equal(Category.DivinationCard, actual.Header.Category);
            Assert.Equal(Rarity.DivinationCard, actual.Header.Rarity);
            Assert.Null(actual.Header.ApiName);
            Assert.Equal("The Lord of Celebration", actual.Header.ApiType);
            Assert.False(actual.Properties.Influences.Crusader);
            Assert.False(actual.Properties.Influences.Elder);
            Assert.False(actual.Properties.Influences.Hunter);
            Assert.False(actual.Properties.Influences.Redeemer);
            Assert.False(actual.Properties.Influences.Shaper);
            Assert.False(actual.Properties.Influences.Warlord);
        }

        [Fact]
        public void BoonOfJustice()
        {
            var actual = parser.ParseItem(@"Item Class: Divination Cards
Rarity: Divination Card
Boon of Justice
--------
Stack Size: 1/6
--------
Offering to the Goddess
--------
Some gifts are obligations while others are simply opportunities.
--------
Note: ~price 1 blessed
");

            Assert.Equal("card", actual.Header.ItemCategory);
            Assert.Equal(Rarity.DivinationCard, actual.Header.Rarity);
            Assert.Equal(Category.DivinationCard, actual.Header.Category);
            Assert.Equal("Boon of Justice", actual.Header.ApiType);
        }
    }
}
