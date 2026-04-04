using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Filters;

[Collection(Collections.Poe1EnglishFixture)]
public class GemFilters(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public async Task VaalGem()
    {
        var item = parser.ParseItem(@"Item Class: Skill Gems
Rarity: Gem
Double Strike
--------
Vaal, Attack, Melee, Strike, Duration, Physical
Level: 1
Mana Cost: 5
Attack Speed: 80% of base
Effectiveness of Added Damage: 91%
Experience: 1/70
--------
Requirements:
Level: 1
--------
Performs two fast strikes with a melee weapon.
--------
Deals 91.3% of Base Damage
25% chance to cause Bleeding
Adds 3 to 5 Physical Damage against Bleeding Enemies
--------
Vaal Double Strike
--------
Souls Per Use: 30
Can Store 2 Uses
Soul Gain Prevention: 8 sec
Effectiveness of Added Damage: 28%
--------
Performs two fast strikes with a melee weapon, each of which summons a double of you for a duration to continuously attack monsters in this fashion.
--------
Deals 28% of Base Damage
Base duration is 6.00 seconds
Modifiers to Skill Effect Duration also apply to this Skill's Soul Gain Prevention
Can't be Evaded
25% chance to cause Bleeding
Adds 3 to 5 Physical Damage against Bleeding Enemies
--------
Place into an item socket of the right colour to gain this skill.Right click to remove from a socket.
--------
Corrupted
--------
Note: ~price 2 chaos
");

        var filters = await fixture.GetPropertyFilters(item);
        Assert.Equal(4, filters.Count);
        Assert.IsType<QualityFilter>(filters[0]);
        Assert.IsType<GemLevelFilter>(filters[1]);
        Assert.IsType<CorruptedFilter>(filters[2]);
        Assert.IsType<ImbuedGemFilter>(filters[3]);
    }

    [Fact]
    public async Task Enlighten()
    {
        var item = parser.ParseItem(@"Item Class: Support Gems
Rarity: Gem
Enlighten Support
--------
Exceptional, Support
Level: 4 (Max)
Cost & Reservation Multiplier: 88%
Quality: +20% (augmented)
--------
Requirements:
Level: 60
Int: 96
--------
Supports any skill gem. Once this gem reaches level 2 or above, will apply a cost & reservation multiplier to supported gems. Cannot support skills that don't come from gems.
--------
This Gem gains 100% increased Experience
--------
This is a Support Gem. It does not grant a bonus to your character, but to skills in sockets connected to it. Place into an item socket connected to a socket containing the Skill Gem you wish to augment. Right click to remove from a socket.
--------
Corrupted
");

        var filters = await fixture.GetPropertyFilters(item);
        Assert.Equal(3, filters.Count);
        Assert.IsType<QualityFilter>(filters[0]);
        Assert.IsType<GemLevelFilter>(filters[1]);
        Assert.IsType<CorruptedFilter>(filters[2]);
    }
}
