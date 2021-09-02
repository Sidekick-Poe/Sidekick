using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class BlightParsing
    {
        private readonly IItemParser parser;

        public BlightParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseBlightedMap()
        {
            var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Blighted Atoll Map
--------
Map Tier: 14
Atlas Region: Tirn's End
--------
Item Level: 81
--------
Area is infested with Fungal Growths (implicit)
Map's Item Quantity Modifiers also affect Blight Chest count at 20% value (implicit)
Natural inhabitants of this area have been removed (implicit)
--------
Travel to this Map by using it in a personal Map Device. Maps can only be used once.
");

            Assert.Equal(Category.Map, actual.Metadata.Category);
            Assert.Equal(Class.Maps, actual.Metadata.Class);
            Assert.Equal(Rarity.Normal, actual.Metadata.Rarity);
            Assert.Equal("Atoll Map", actual.Metadata.Type);
            Assert.Equal(14, actual.Properties.MapTier);
            Assert.True(actual.Properties.Blighted);
        }

        [Fact]
        public void ClearOil()
        {
            var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Clear Oil
--------
Stack Size: 5/10
--------
Can be combined with other Oils at Cassia to Enchant Rings or Amulets, or to modify Blighted Maps.
Shift click to unstack.
--------
Note: ~price 1 blessed
");

            Assert.Equal(Class.StackableCurrency, actual.Metadata.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Clear Oil", actual.Metadata.Type);
        }

    }
}
