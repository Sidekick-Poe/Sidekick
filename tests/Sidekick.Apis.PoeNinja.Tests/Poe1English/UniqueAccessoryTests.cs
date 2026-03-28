using System.Threading.Tasks;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.PoeNinja.Tests.Poe1English;

[Collection(Collections.NinjaTestCollection)]
public class UniqueAccessoryTests(NinjaTestFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public async Task AtzirisFoible()
    {
        var item = parser.ParseItem(@"Item Class: Amulets
Rarity: Unique
Atziri's Foible
Paua Amulet
--------
Requirements:
Level: 16
--------
Item Level: 83
--------
Allocates Mind Drinker (enchant)
--------
30% increased Mana Regeneration Rate (implicit)
--------
+100 to maximum Mana
22% increased maximum Mana
83% increased Mana Regeneration Rate
Items and Gems have 25% reduced Attribute Requirements
--------
The world is but a piece of parchment, blank and symmetric. 
We label each side: one Good, one Evil; one Black, one White. 
The divine truth, however, is that both are one and the same. 
-Jaetai, Vaal Advisor

");

        var results = await fixture.NinjaStashProvider.GetInfo(item);
        Assert.Single(results);

        var result = results[0];
        Assert.Equal("atziris-foible-paua-amulet", result.DetailsId);
    }
}
