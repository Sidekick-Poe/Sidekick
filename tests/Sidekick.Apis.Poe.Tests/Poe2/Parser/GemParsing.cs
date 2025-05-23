using Sidekick.Apis.Poe.Trade;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe2.Parser;

[Collection(Collections.Poe2Parser)]
public class GemParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseUncutSpirit()
    {
        var actual = parser.ParseItem(
            @"Rarity: Currency
Uncut Spirit Gem
--------
Level: 14
--------
Item Level: 14
--------
Creates a Persistent Buff Skill Gem or Level an existing gem to Level 14
--------
Right Click to engrave a Persistent Buff Skill Gem.
");

        Assert.Equal(Category.Gem, actual.Header.Category);
        Assert.Equal("Uncut Spirit Gem", actual.Header.ApiType);
        Assert.Null(actual.Header.ApiName);
        Assert.Equal(14, actual.Properties.GemLevel);
    }

    [Fact]
    public void ParseSupport3()
    {
        var actual = parser.ParseItem(
            @"Rarity: Currency
Uncut Support Gem
--------
Level: 3
--------
Item Level: 3
--------
Creates a Support Gem up to level 3
--------
Right Click to engrave a Support Gem.
");

        Assert.Equal(Category.Gem, actual.Header.Category);
        Assert.Equal("Uncut Support Gem", actual.Header.ApiType);
        Assert.Null(actual.Header.ApiName);
        Assert.Equal(3, actual.Properties.GemLevel);
    }

    [Fact]
    public void ParseLightningArrow()
    {
        var actual = parser.ParseItem(
            @"Item Class: Skill Gems
Rarity: Gem
Herald of Ice
--------
Buff, Attack, Persistent, AoE, Cold, Herald, Payoff
Level: 18
Quality: +20% (augmented)
Attack Damage: 247%
--------
Requires: Level 78, 97 Dex, 97 Int
Requires: Any Martial Weapon
--------
Sockets: G G G G 
--------
While active, Shattering an enemy with an Attack Hit will cause an icy explosion that deals Attack damage to surrounding enemies.
--------
Explosion
--------
Explosion radius is 2.2 metres
100% of Explosion Physical Damage
Converted to Cold Damage
--------
Buff
--------
Reservation: 30 Spirit
--------
Enemies you Shatter explode
--------
Skills can be managed in the Skills Panel.
");

        Assert.Equal(Category.Gem, actual.Header.Category);
        Assert.Equal("Herald of Ice", actual.Header.ApiType);
        Assert.Null(actual.Header.ApiName);
        Assert.Equal(18, actual.Properties.GemLevel);
        Assert.NotNull(actual.Properties.Sockets);
        Assert.Equal(4, actual.Properties.Sockets.Count);
    }
}
