using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser
{
    [Collection(Collections.Poe1Parser)]
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
            Assert.Equal(Class.SanctumResearch, actual.Header.Class);
            Assert.Equal("Forbidden Tome", actual.Metadata.Type);
            Assert.Equal(83, actual.Properties.AreaLevel);
        }
    }
}
