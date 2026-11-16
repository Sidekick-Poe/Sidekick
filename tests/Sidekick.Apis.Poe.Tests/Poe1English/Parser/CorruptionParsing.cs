using Sidekick.Game.Parser;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Properties.Definitions;
using Sidekick.Game.Parser.Stats;
using Sidekick.Game.Parser.Trade.Requests;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class CorruptionParsing(Poe1EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public async Task TwiceCorruptedRing()
    {
        var actual = parser.ParseItem(@"Item Class: Rings
Rarity: Rare
Brood Circle
Ruby Ring
--------
Requirements:
Level: 36
--------
Item Level: 76
--------
Anger has 18% increased Aura Effect (implicit)
--------
+16 to all Attributes
+31 to Intelligence
Adds 8 to 13 Physical Damage to Attacks
31% increased Mana Regeneration Rate
--------
Twice Corrupted
");

        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.False(actual.Properties.Corrupted);
        Assert.True(actual.Properties.TwiceCorrupted);
        Assert.Equal(5, actual.Stats.Count);
        fixture.AssertHasStat(actual, StatCategory.Implicit, "Anger has #% increased Aura Effect", 18);

        var filters = await fixture.GetPropertyFilters(actual);
        var corrupted = Assert.Single(filters.OfType<CorruptedFilter>());
        var twiceCorrupted = Assert.Single(filters.OfType<TwiceCorruptedFilter>());
        Assert.True(corrupted.Checked);
        Assert.True(twiceCorrupted.Checked);

        var query = new Query();
        corrupted.PrepareTradeRequest(query, actual);
        twiceCorrupted.PrepareTradeRequest(query, actual);
        var miscFilters = query.Filters.GetOrCreateMiscFilters().Filters;
        Assert.Equal("true", miscFilters.Corrupted?.Option);
        Assert.Equal("true", miscFilters.TwiceCorrupted?.Option);
    }
}
