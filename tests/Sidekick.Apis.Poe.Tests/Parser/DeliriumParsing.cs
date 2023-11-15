using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class DeliriumParsing
    {
        private readonly IItemParser parser;

        public DeliriumParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void SimulacrumSplinter()
        {
            var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Simulacrum Splinter
--------
Stack Size: 40/300
--------
Combine 300 Splinters to create a Simulacrum.
Shift click to unstack.
--------
Note: ~price .5 chaos
");

            Assert.Equal(Class.StackableCurrency, actual.Header.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Simulacrum Splinter", actual.Metadata.Type);
        }

        [Fact]
        public void SmallClusterJewel()
        {
            var actual = parser.ParseItem(@"Item Class: Jewels
Rarity: Rare
Oblivion Ruin
Small Cluster Jewel
--------
Item Level: 45
--------
Adds 2 Passive Skills (enchant)
Added Small Passive Skills grant: 15% increased Evasion Rating (enchant)
--------
Added Small Passive Skills also grant: +3 to Maximum Life
Added Small Passive Skills also grant: +3 to Strength
1 Added Passive Skill is Readiness
--------
Place into an allocated Small, Medium or Large Jewel Socket on the Passive Skill Tree. Added passives do not interact with jewel radiuses. Right click to remove from the Socket.
--------
Note: ~b/o 1 chance
");

            Assert.Equal(Class.Jewel, actual.Header.Class);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal(Category.Jewel, actual.Metadata.Category);
            Assert.Equal("Small Cluster Jewel", actual.Metadata.Type);
        }

    }
}
