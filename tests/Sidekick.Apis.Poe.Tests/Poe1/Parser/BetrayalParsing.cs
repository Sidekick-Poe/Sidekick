using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
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

        Assert.Equal(ItemClass.MapFragment, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Equal(Category.Map, actual.Header.Category);
        Assert.Equal("Rusted Reliquary Scarab", actual.Header.ApiType);
    }

}
