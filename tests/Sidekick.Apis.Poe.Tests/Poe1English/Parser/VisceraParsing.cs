using Sidekick.Game.Parser;
using Sidekick.Game.Parser.Stats;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class VisceraParsing(Poe1EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void GeneralsHead()
    {
        var actual = parser.ParseItem(@"Item Class: Viscera
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

        Assert.Equal("Viscera", actual.ItemClass.Id);
        Assert.Equal("General's Head", actual.Type);
        Assert.Equal("General's Head", actual.TradeItem?.Type);
        Assert.Equal(78, actual.Properties.ItemLevel);
        fixture.AssertHasStat(actual, StatCategory.Implicit, "#% increased maximum Energy Shield", 18);
    }

    [Fact]
    public void LesserAristocratsTorso()
    {
        var actual = parser.ParseItem(@"Item Class: Viscera
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

        Assert.Equal("Viscera", actual.ItemClass.Id);
        Assert.Equal("Lesser Aristocrat's Torso", actual.Type);
        Assert.Equal("Aristocrat's Torso", actual.TradeItem?.Type);
        fixture.AssertHasStat(actual, StatCategory.Implicit, "#% increased Life Regeneration rate", 8);
    }

    [Fact]
    public void GreaterArachnidEggSac()
    {
        var actual = parser.ParseItem(@"Item Class: Viscera
Greater Arachnid Egg Sac
--------
Torso
--------
Item Level: 73
--------
{ Implicit Modifier }
Minions created Recently have Onslaught — Unscalable Value
--------
Take this to the Wickerman in a town or hideout to attach it.
");

        Assert.Equal("Viscera", actual.ItemClass.Id);
        Assert.Equal("Greater Arachnid Egg Sac", actual.Type);
        Assert.Equal("Arachnid Egg Sac", actual.TradeItem?.Type);
        fixture.AssertHasStat(actual, StatCategory.Implicit, "Minions created Recently have Onslaught");
    }

    [Fact]
    public void CovenQueensCrownWithTwoAffixes()
    {
        var actual = parser.ParseItem(@"Item Class: Viscera
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

        Assert.Equal("Viscera", actual.ItemClass.Id);
        Assert.Equal("Coven Queen's Crown", actual.Type);
        fixture.AssertHasStat(actual, StatCategory.Implicit, "Travel Skills have #% chance to not consume a Cooldown on use", 17);
        fixture.AssertHasStat(actual, StatCategory.Implicit, "#% reduced Physical Damage taken over time", -7);
    }
}
