using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class GemParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseVaalGem()
    {
        var actual = parser.ParseItem(@"Item Class: Unknown
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

        Assert.Equal(Category.Gem, actual.Header.Category);
        Assert.Equal(Rarity.Gem, actual.Header.Rarity);
        Assert.Equal("Vaal Double Strike", actual.Header.ApiType);
        Assert.Equal(1, actual.Properties.GemLevel);
        Assert.Equal(0, actual.Properties.Quality);
        Assert.True(actual.Properties.Corrupted);
    }

    [Fact]
    public void ArcaneSurgeSupport()
    {
        var actual = parser.ParseItem(@"Item Class: Support Gems
Rarity: Gem
Arcane Surge Support
--------
Arcane, Support, Spell, Duration
Level: 1
Cost & Reservation Multiplier: 130%
--------
Requirements:
Level: 1
--------
Each supported spell will track how much mana you spend on it, granting a buff when the total mana spent reaches a threshold. Cannot support skills used by totems, traps, mines or skills with a reservation.
--------
Gain Arcane Surge after Spending a total of 15 Mana on
Upfront Costs and Effects of a Supported Skill
Arcane Surge grants 10% increased Cast Speed
Arcane Surge grants 30% increased Mana Regeneration rate
Arcane Surge lasts 4 seconds
Supported Skills deal 10% more Spell Damage while you have Arcane Surge
--------
Experience: 1/70
--------
This is a Support Gem. It does not grant a bonus to your character, but to skills in sockets connected to it. Place into an item socket connected to a socket containing the Skill Gem you wish to augment. Right click to remove from a socket.
");

        Assert.Equal("gem.supportgem", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Gem, actual.Header.Rarity);
        Assert.Equal(Category.Gem, actual.Header.Category);
        Assert.Equal("Arcane Surge Support", actual.Header.ApiType);
    }

    [Fact]
    public void VoidSphere()
    {
        var actual = parser.ParseItem(@"Item Class: Skill Gems
Rarity: Gem
Void Sphere
--------
Spell, AoE, Duration, Physical, Chaos, Orb
Level: 1
Cost: 30 Mana
Cooldown Time: 10.00 sec
Cast Time: 0.60 sec
Critical Strike Chance: 5.00%
Effectiveness of Added Damage: 75%
--------
Requirements:
Level: 34
Int: 79
--------
Creates a Void Sphere which Hinders enemies in an area around it, with the debuff being stronger on enemies closer to the sphere. It also regularly releases pulses of area damage. The Void Sphere will consume the corpses of any enemies which die in its area. Can only have one Void Sphere at a time.
--------
Deals 27 to 40 Physical Damage
Base duration is 5.00 seconds
Pulses every 0.40 seconds
40% of Physical Damage Converted to Chaos Damage
Enemies in range are Hindered, with up to 30% reduced Movement Speed, based on distance from the Void Sphere
--------
Experience: 1/252,595
--------
Place into an item socket of the right colour to gain this skill. Right click to remove from a socket.
");

        Assert.Equal("gem.activegem", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Gem, actual.Header.Rarity);
        Assert.Equal(Category.Gem, actual.Header.Category);
        Assert.Equal("Void Sphere", actual.Header.ApiType);
    }

    [Fact]
    public void TransfiguredGem()
    {
        var actual = parser.ParseItem(@"Item Class: Skill Gems
Rarity: Gem
Kinetic Blast of Clustering
--------
Attack, Projectile, AoE, Physical
Level: 4
Cost: 7 Mana
Attack Speed: 115% of base
Attack Damage: 100% of base
--------
Requirements:
Level: 37
Int: 85
--------
Fires a projectile from a Wand that causes a series of area explosions in a secondary radius around its point of impact, each damaging enemies.
--------
Kinetic Blast creates 3 additional explosions
Increases and Reductions to Spell Damage also apply to Attack Damage from this Skill at 200% of their value
Deals Added Physical Damage equal to 9% of Maximum Mana
Modifiers to number of Projectiles instead apply to the number of Explosions
Base explosion radius is 1.9 metres
Base secondary radius is 2.8 metres
Projectiles cannot Split
--------
Experience: 127,372/554,379
--------
Place into an item socket of the right colour to gain this skill. Right click to remove from a socket.
--------
Transfigured
");

        Assert.Equal("gem.activegem", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Gem, actual.Header.Rarity);
        Assert.Equal(Category.Gem, actual.Header.Category);
        Assert.Equal("Kinetic Blast of Clustering", actual.Header.ApiText);
        Assert.Equal("Kinetic Blast", actual.Header.ApiType);
        Assert.Equal("alt_x", actual.Header.ApiDiscriminator);
    }
}
