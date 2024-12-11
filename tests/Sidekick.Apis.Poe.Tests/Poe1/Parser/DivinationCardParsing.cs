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
            Assert.Equal(Category.DivinationCard, actual.Metadata.Category);
            Assert.Equal(Rarity.DivinationCard, actual.Metadata.Rarity);
            Assert.Null(actual.Metadata.Name);
            Assert.Equal("The Saint's Treasure", actual.Metadata.Type);
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
            Assert.Equal(Category.DivinationCard, actual.Metadata.Category);
            Assert.Equal(Rarity.DivinationCard, actual.Metadata.Rarity);
            Assert.Null(actual.Metadata.Name);
            Assert.Equal("The Lord of Celebration", actual.Metadata.Type);
            Assert.False(actual.Influences.Crusader);
            Assert.False(actual.Influences.Elder);
            Assert.False(actual.Influences.Hunter);
            Assert.False(actual.Influences.Redeemer);
            Assert.False(actual.Influences.Shaper);
            Assert.False(actual.Influences.Warlord);
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
            Assert.Equal(Rarity.DivinationCard, actual.Metadata.Rarity);
            Assert.Equal(Category.DivinationCard, actual.Metadata.Category);
            Assert.Equal("Boon of Justice", actual.Metadata.Type);
        }
    }
}
