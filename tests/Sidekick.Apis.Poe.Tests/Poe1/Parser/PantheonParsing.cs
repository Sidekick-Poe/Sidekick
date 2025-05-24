using Sidekick.Apis.Poe.Trade;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class PantheonParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void DivineVessel()
    {
        var actual = parser.ParseItem(@"Item Class: Map Fragments
Rarity: Normal
Divine Vessel
--------
Unique Boss deals 10% increased Damage
Unique Boss has 10% increased Attack and Cast Speed
Unique Boss has 10% increased Life
Unique Boss has 20% increased Area of Effect
--------
Power is a curious thing. 
It can be contained, hidden, locked away, 
and yet it always breaks free.
--------
Can be used in a personal Map Device, allowing you to capture the Soul of the Map's Boss. The Vessel containing the captured Soul can be retrieved from the Map Device. You must be in the Map when the boss is defeated.
--------
Note: ~price 1 chaos
");

        Assert.Equal("map.fragment", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Normal, actual.Header.Rarity);
        Assert.Equal(Category.Map, actual.Header.Category);
        Assert.Equal("Divine Vessel", actual.Header.ApiType);
    }
}
