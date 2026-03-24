using System.Threading.Tasks;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.PoeNinja.Tests.Poe1English;

[Collection(Collections.NinjaTestCollection)]
public class ClusterJewelTests(NinjaTestFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public async Task SmallCluster()
    {
        var item = parser.ParseItem(@"Item Class: Jewels
Rarity: Rare
Blight Ruin
Small Cluster Jewel
--------
Requirements:
Level: 62
--------
Item Level: 83
--------
Adds 3 Passive Skills (enchant)
Added Small Passive Skills grant: 6% increased Mana Reservation Efficiency of Skills (enchant)
--------
Added Small Passive Skills also grant: +6% to Fire Resistance
Added Small Passive Skills also grant: 8% increased Mana Regeneration Rate
Added Small Passive Skills also grant: +7 to Maximum Life
--------
Place into an allocated Small, Medium or Large Jewel Socket on the Passive Skill Tree. Added passives do not interact with jewel radiuses. Right click to remove from the Socket.
");

        var result = await fixture.NinjaStashProvider.GetInfo(item);

        Assert.NotNull(result);
        Assert.Equal("6-increased-mana-reservation-efficiency-of-skills-3-passives-75", result.DetailsId);
    }

}
