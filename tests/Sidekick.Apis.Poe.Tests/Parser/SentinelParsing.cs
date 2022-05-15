using System.Linq;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class SentinelParsing
    {
        private readonly IItemParser parser;

        public SentinelParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseRareSentinel()
        {
            var actual = parser.ParseItem(@"Item Class: Sentinel
Rarity: Rare
Ominous Sentry
Bronze Stalker Sentinel
--------
Duration: 36 (augmented) seconds
Empowers: 30 (augmented) enemies
Empowerment: 18 (augmented)
--------
Requirements:
Level: 5
--------
Item Level: 12
--------
+3 to Charge
22% increased Duration
20% increased number of Empowered Enemies
3.0% chance for Empowered Enemies to have a Jewellery Reward
Empowered Monsters have 58% increased Rarity of Items Found
Empowered Monsters grant 52% increased Sentinel Power
--------
Charge: 0/13 (augmented) (unmet)
--------
Unmodifiable
");

            Assert.Equal(Class.Sentinel, actual.Metadata.Class);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal(Category.Sentinel, actual.Metadata.Category);
            Assert.Equal("Bronze Stalker Sentinel", actual.Metadata.Type);
        }
    }
}
