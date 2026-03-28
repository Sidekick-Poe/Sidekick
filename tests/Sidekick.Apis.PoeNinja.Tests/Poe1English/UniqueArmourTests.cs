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

        var results = await fixture.NinjaStashProvider.GetInfo(item);
        Assert.Single(results);

        var result = results[0];
        Assert.Equal("foulborn-ghostwrithe-chaos-recoup-life-to-shield-silken-vest-6l", result.DetailsId);
    }

    [Fact]
    public async Task FoulbornSquire()
    {
        var item = parser.ParseItem(@"Item Class: Shields
Rarity: Unique
Foulborn The Squire
Elegant Round Shield
--------
Chance to Block: 29% (augmented)
Armour: 345 (augmented)
Evasion Rating: 345 (augmented)
--------
Requirements:
Level: 70
Str: 85
Dex: 85
--------
Sockets: W-W-W 
--------
Item Level: 83
--------
120% increased Block Recovery (implicit)
--------
Has 3 Sockets
Socketed Support Gems can also Support Skills from your Main Hand
113% increased Armour and Evasion
+4% Chance to Block
+1 to Level of Socketed Gems (mutated)
Ignore Attribute Requirements of Socketed Gems (mutated)
--------
Judge not the weak, for
they empower the strong.
--------
Note: ~b/o 280 divine
");

        var results = await fixture.NinjaStashProvider.GetInfo(item);
        Assert.Single(results);

        var result = results[0];
        Assert.Equal("foulborn-the-squire-gem-level-no-requirements-elegant-round-shield", result.DetailsId);
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

        var results = await fixture.NinjaStashProvider.GetInfo(item);
        Assert.Single(results);

        var result = results[0];
        Assert.Equal("foulborn-rathpith-globe-dot-per-life-titanium-spirit-shield", result.DetailsId);
    }
}
