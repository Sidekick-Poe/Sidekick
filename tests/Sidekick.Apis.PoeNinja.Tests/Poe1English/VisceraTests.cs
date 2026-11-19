using System.Linq;
using Sidekick.Game.Parser;
using Xunit;
namespace Sidekick.Apis.PoeNinja.Tests.Poe1English;

[Collection(Collections.NinjaPoe1TestCollection)]
public class VisceraTests(NinjaPoe1TestFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void GeneralsHead()
    {
        var item = parser.ParseItem(@"Item Class: Viscera
General's Head
--------
Head
--------
Item Level: 78
--------
{ Implicit Modifier }
18(15-20)% increased maximum Energy Shield
--------
Take this to the Wickerman in a town or hideout to attach it.
");

        fixture.AssertStash(item, "generals-head-increased-maximum-energy-shield");
    }

    [Fact]
    public void CovenQueensCrownWithTwoAffixes()
    {
        var item = parser.ParseItem(@"Item Class: Viscera
Coven Queen's Crown
--------
Head
--------
Item Level: 85
--------
{ Implicit Modifier }
Travel Skills have 17(15-20)% chance to not consume a Cooldown on use
{ Implicit Modifier }
7(7-6)% reduced Physical Damage taken over time
--------
Sinew cleft from bone, the crown was placed atop her head,
eyes alight with shadow. In the gloom, the gathered witches
whispered reverence to the first and last Queen of Umbra.
--------
Take this to the Wickerman in a town or hideout to attach it.
");

        var results = fixture.NinjaStashProvider.GetDefinitions(item).Select(x => x.DetailsId).ToList();
        Assert.Equal(2, results.Count);
        Assert.Contains("coven-queens-crown-travel-skills-have-chance-to-not-consume-a-cooldown-on-use", results);
        Assert.Contains("coven-queens-crown-reduced-physical-damage-taken-over-time", results);
    }

    [Fact]
    public void LesserVisceraHasNoPriceYet()
    {
        var item = parser.ParseItem(@"Item Class: Viscera
Lesser Aristocrat's Torso
--------
Torso
--------
Item Level: 32
--------
{ Implicit Modifier }
8(8-10)% increased Life Regeneration rate
--------
Take this to the Wickerman in a town or hideout to attach it.
");

        Assert.Empty(fixture.NinjaStashProvider.GetDefinitions(item));
    }
}
