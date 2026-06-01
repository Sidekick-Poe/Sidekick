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

    [Fact]
    public async Task FoulbornKhatalWeeping()
    {
        var item = parser.ParseItem(@"Item Class: Amulets
Rarity: Unique
Foulborn Khatal's Weeping
Lapis Amulet
--------
Requirements:
Level: 56
--------
Item Level: 85
--------
+21 to Intelligence (implicit)
--------
Life Flasks gain 3 Charges every 3 seconds (mutated)
+88 to maximum Life
Can't use Mana Flasks
On non-channelling Attack, set a Life Flask with greater than 50% of maximum Charges remaining to 50%
For each Charge removed this way, that Attack gains +2% to Damage over time Multiplier
--------
""We fought the Abyssals, too. That's the part they leave out.
Khatal was Faridun... and for saving all their lives, the Maraketh
made him drink the enemy's blood. He melted from the inside out.""
- Toryal, of the Afarud
");

        var results = await fixture.NinjaStashProvider.GetInfo(item);
        Assert.Single(results);

        Assert.Contains(results, x => x.DetailsId == "foulborn-khatals-weeping-life-flask-charge-lapis-amulet");
    }
}
