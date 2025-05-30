using Sidekick.Apis.Poe.Trade;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class BetrayalParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void RustedReliquaryScarab()
    {
        var actual = parser.ParseItem(@"Item Class: Map Fragments
Rarity: Normal
Rusted Reliquary Scarab
--------
50% more Unique Items found in Area
--------
The Maraketh left you to die alone in the desert, young Sumei, but we
saw the potential in you. The Order of the Djinn is your akhara now.
--------
Can be used in a personal Map Device to add modifiers to a Map.
--------
Note: ~b/o .50 chaos
");

        Assert.Equal("map.fragment", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Normal, actual.Header.Rarity);
        Assert.Equal(Category.Map, actual.Header.Category);
        Assert.Equal("Rusted Reliquary Scarab", actual.Header.ApiType);
    }

}
