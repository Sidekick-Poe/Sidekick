using System.Threading.Tasks;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.PoeNinja.Tests.Poe1English;

[Collection(Collections.NinjaTestCollection)]
public class UniqueArmourTests(NinjaTestFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public async Task FoulbornGhostwrithe()
    {
        var item = parser.ParseItem(@"Item Class: Body Armours
Rarity: Unique
Foulborn Ghostwrithe
Silken Vest
--------
Quality: +20% (augmented)
Energy Shield: 160 (augmented)
--------
Requirements:
Level: 11
Int: 37
--------
Sockets: B-B-B-B-B-B 
--------
Item Level: 83
--------
+101 to maximum Energy Shield
+79 to maximum Life
50% of Chaos Damage taken Recouped as Life (mutated)
Your Maximum Energy Shield is Equal to 40% of Your Maximum Life (mutated)
--------
Faith springs abundant at the edge of death.
--------
Note: ~b/o 400 chaos
");

        var result = await fixture.NinjaStashProvider.GetInfo(item);

        Assert.NotNull(result);
        Assert.Equal("foulborn-ghostwrithe-chaos-recoup-life-to-shield-silken-vest-6l", result.DetailsId);
    }

    [Fact]
    public async Task FoulbornRathpithGlobe()
    {
        var item = parser.ParseItem(@"Item Class: Shields
Rarity: Unique
Foulborn Rathpith Globe
Titanium Spirit Shield
--------
Chance to Block: 25%
Energy Shield: 167 (augmented)
--------
Requirements:
Level: 68
Int: 159
--------
Sockets: R-R-G 
--------
Item Level: 85
--------
14% Chance to Block Spell Damage
122% increased Energy Shield
10% increased maximum Life
Sacrifice 10% of your Life when you Use or Trigger a Spell Skill
5% increased Spell Damage per 100 Player Maximum Life
Deal 5% increased Damage Over Time per 100 Player Maximum Life (mutated)
--------
The Vaal emptied their slaves of beating hearts,
and left a mountain of twitching dead.
--------
Note: ~b/o 250 divine
");

        var result = await fixture.NinjaStashProvider.GetInfo(item);

        Assert.NotNull(result);
        Assert.Equal("foulborn-rathpith-globe-dot-per-life-titanium-spirit-shield", result.DetailsId);
    }
}
