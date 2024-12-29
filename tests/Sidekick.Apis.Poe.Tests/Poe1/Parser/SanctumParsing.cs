using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser
{
    [Collection(Collections.Poe1Parser)]
    public class SanctumParsing(ParserFixture fixture)
    {
        private readonly IItemParser parser = fixture.Parser;

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

            Assert.Equal(Category.Sanctum, actual.Header.Category);
            Assert.Equal("sanctum.research", actual.Header.ItemCategory);
            Assert.Equal("Forbidden Tome", actual.Header.ApiType);
            Assert.Equal(83, actual.Properties.AreaLevel);
        }
    }
}
