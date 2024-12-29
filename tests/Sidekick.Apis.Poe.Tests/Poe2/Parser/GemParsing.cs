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
}
