using System.Threading.Tasks;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.PoeNinja.Tests.Poe1English;

[Collection(Collections.NinjaTestCollection)]
public class GemTests(NinjaTestFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public async Task ShockNovaOfProcession()
    {
        var item = parser.ParseItem(@"Item Class: Skill Gems
Rarity: Gem
Shock Nova of Procession
--------
Spell, AoE, Lightning, Nova
Level: 20 (Max)
Cost: 23 Mana
Cast Time: 0.25 sec
Critical Strike Chance: 6.00%
Effectiveness of Added Damage: 95%
Quality: +20% (augmented)
--------
Requirements:
Level: 70
Int: 155
--------
Casts a ring of Lightning around you, followed by a larger Lightning nova. Each effect hits enemies caught in their area with Lightning Damage. This then repeats, with each repeat offset from the previous in the direction you target.
--------
Deals 263 to 790 Lightning Damage
Repeats 2 times
10% more Cast Speed
30% less Area of Effect
--------
Place into an item socket of the right colour to gain this skill. Right click to remove from a socket.
--------
Corrupted
--------
Imbued
--------
Transfigured
");

        var results = await fixture.NinjaStashProvider.GetInfo(item);
        Assert.Single(results);

        var result = results[0];
        Assert.Equal("shock-nova-of-procession-20-20c", result.DetailsId);
    }
}
