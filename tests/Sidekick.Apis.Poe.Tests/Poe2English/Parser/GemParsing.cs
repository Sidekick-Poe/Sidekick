using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe2English.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class GemParsing(Poe2EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseUncutSpirit16()
    {
        var actual = parser.ParseItem(
            @"Item Class: Uncut Spirit Gems
Rarity: Currency
Uncut Spirit Gem (Level 16)
--------
Creates a Persistent Buff Skill Gem or Level an existing gem to Level 16
--------
Right Click to engrave a Persistent Buff Skill Gem.
");

        Assert.Equal(ItemClass.UncutSpiritGem, actual.Properties.ItemClass);
        Assert.Equal(Category.Gem, actual.ApiInformation.Category);
        Assert.Equal("Uncut Spirit Gem (Level 16)", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);
    }

    [Fact]
    public void ParseSupport3()
    {
        var actual = parser.ParseItem(
            @"Item Class: Uncut Support Gems
Rarity: Currency
Uncut Support Gem (Level 3)
--------
Creates a Support Gem
--------
Right Click to engrave a Support Gem.
");

        Assert.Equal(ItemClass.UncutSupportGem, actual.Properties.ItemClass);
        Assert.Equal(Category.Gem, actual.ApiInformation.Category);
        Assert.Equal("Uncut Support Gem (Level 3)", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);
    }

    [Fact]
    public void ParseSkill9()
    {
        var actual = parser.ParseItem(
            @"Item Class: Uncut Skill Gems
Rarity: Currency
Uncut Skill Gem (Level 9)
--------
Creates a Skill Gem or Level an existing gem to level 9
--------
Right Click to engrave a Skill Gem.
");

        Assert.Equal(ItemClass.UncutSkillGem, actual.Properties.ItemClass);
        Assert.Equal(Category.Gem, actual.ApiInformation.Category);
        Assert.Equal("Uncut Skill Gem (Level 9)", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);
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

        Assert.Equal(Category.Gem, actual.ApiInformation.Category);
        Assert.Equal("Herald of Ice", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);
        Assert.Equal(18, actual.Properties.GemLevel);
        Assert.NotNull(actual.Properties.Sockets);
        Assert.Equal(4, actual.Properties.Sockets.Count);
    }
}
