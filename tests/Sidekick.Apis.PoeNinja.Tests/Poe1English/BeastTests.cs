using System.Threading.Tasks;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.PoeNinja.Tests.Poe1English;

[Collection(Collections.NinjaTestCollection)]
public class BeastTests(NinjaTestFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public async Task Tier16()
    {
        var item = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Rare
Hairyjoint
Vivid Vulture
--------
Genus: Vultures
Group: Avians
Family: The Sands
--------
Item Level: 83
--------
Saqawine Presence
Crimson Flock
Raven Caller
Heals Allies and Suppresses Foe Recovery
Temporarily Revives
Hasted
Cannot be Stunned
10% chance not to be consumed when sacrificed at the Blood Altar
--------
Right-click to add this to your bestiary.
--------
Note: ~b/o 450 chaos
");

        var results = await fixture.NinjaStashProvider.GetInfo(item);
        Assert.Single(results);

        var result = results[0];
        Assert.Equal($"vivid-vulture", result.DetailsId);
    }
}
