using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Items.Modifiers;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class SanctumParsing
    {
        private readonly IItemParser parser;

        public SanctumParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseFloor()
        {
            var actual = parser.ParseItem(@"Item Class: Sanctum Research
Rarity: Currency
Forbidden Tome
--------
Area Level: 83
--------
Item Level: 84
--------
Mundus noster cecidit. Daemones ubique sunt. Librum
hunc in sacrarium conicio, ut forte alius viam inveniat...
--------
Take this item to the Relic Altar in the Forbidden Sanctum to enter.
");

            Assert.Equal(Category.Sanctum, actual.Metadata.Category);
            Assert.Equal(Class.SanctumResearch, actual.Metadata.Class);
            Assert.Equal("Forbidden Tome", actual.Metadata.Type);
            Assert.Equal(83, actual.Properties.AreaLevel);
        }
    }
}
