using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Properties.Definitions;
using Sidekick.Game.Parser.Stats;
using Sidekick.Game.Parser.Trade.Requests;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe2.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class CorruptionParsing(Poe2EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    private const string Headhunter = @"Item Class: Belts
Rarity: Unique
Headhunter
Heavy Belt
--------
Requires: Level 50
--------
Item Level: 82
--------
+25% to Lightning Resistance (enchant)
--------
{ Implicit Modifier }
23(20-30)% increased Stun Threshold
{ Implicit Modifier — Charm }
Has 2(1-3) Charm Slots
--------
{ Unique Modifier — Attribute }
+37(20-40) to Strength
{ Unique Modifier — Attribute }
+25(20-40) to Dexterity
{ Unique Modifier — Life }
+60(40-60) to maximum Life
{ Unique Modifier }
When you kill a Rare monster, you gain its Modifiers for 60 seconds
--------
""A man's soul rules from a cavern of bone, learns and
judges through flesh-born windows. The heart is meat.
The head is where the Man is.""
Lavianga, Advisor to Kaom
";

    private static string Copy(string? corruption)
    {
        var text = Headhunter;
        if (corruption != null) text += "--------\n" + corruption + "\n";
        return text + "--------\nNote: ~b/o 119 divine\n";
    }

    [Fact]
    public async Task CorruptedHeadhunter()
    {
        var actual = parser.ParseItem(Copy("Corrupted"));

        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.True(actual.Properties.Corrupted);
        Assert.False(actual.Properties.TwiceCorrupted);
        fixture.AssertHasStat(actual, StatCategory.Enchant, "#% to Lightning Resistance", 25);

        var filters = await fixture.GetPropertyFilters(actual);
        Assert.True(Assert.Single(filters.OfType<CorruptedFilter>()).Checked);
        var twiceCorrupted = Assert.Single(filters.OfType<TwiceCorruptedFilter>());
        Assert.Null(twiceCorrupted.Checked);
    }

    [Fact]
    public async Task TwiceCorruptedHeadhunter()
    {
        var actual = parser.ParseItem(Copy("Twice Corrupted"));

        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.False(actual.Properties.Corrupted);
        Assert.True(actual.Properties.TwiceCorrupted);
        fixture.AssertHasStat(actual, StatCategory.Enchant, "#% to Lightning Resistance", 25);

        var filters = await fixture.GetPropertyFilters(actual);
        var corrupted = Assert.Single(filters.OfType<CorruptedFilter>());
        Assert.True(corrupted.Checked);
        var twiceCorrupted = Assert.Single(filters.OfType<TwiceCorruptedFilter>());
        Assert.Null(twiceCorrupted.Checked);
        Assert.Equal("Twice Corrupted", twiceCorrupted.Text);

        var query = new Query();
        corrupted.PrepareTradeRequest(query, actual);
        twiceCorrupted.PrepareTradeRequest(query, actual);
        var miscFilters = query.Filters.GetOrCreateMiscFilters().Filters;
        Assert.Equal("true", miscFilters.Corrupted?.Option);
        Assert.Null(miscFilters.TwiceCorrupted?.Option);
    }
}
