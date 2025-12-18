using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe2English.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class FlaskParsing(Poe2EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseLifeFlask()
    {
        var actual = parser.ParseItem(
            @"Item Class: Life Flasks
Rarity: Magic
Simmering Ultimate Life Flask of the Distiller
--------
Recovers 920 Life over 3 Seconds
Consumes 7 (augmented) of 75 Charges on use
Currently has 0 Charges
--------
Requirements:
Level: 60
--------
Item Level: 66
--------
23% of Recovery applied Instantly
26% reduced Charges per use
--------
Right click to drink. Can only hold charges while in belt. Refill at Wells or by killing monsters.");

        Assert.Equal(ItemClass.LifeFlask, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Ultimate Life Flask", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);

        Assert.Equal(66, actual.Properties.ItemLevel);

        actual.AssertHasModifier(ModifierCategory.Explicit, "#% of Recovery applied Instantly", 23);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Charges per use", -26);
        Assert.False(actual.Modifiers[1].MatchedFuzzily);
    }

    [Fact]
    public void ParseCharm()
    {
        var actual = parser.ParseItem(
            @"Item Class: Charms
Rarity: Magic
Analyst's Stone Charm of the Practitioner
--------
Lasts 3.60 (augmented) Seconds
Consumes 16 (augmented) of 40 Charges on use
Currently has 40 Charges
Cannot be Stunned
--------
Requires: Level 16
--------
Item Level: 51
--------
Used when you become Stunned (implicit)
--------
21% increased Duration
19% reduced Charges per use
--------
Used automatically when condition is met. Can only hold charges while in belt. Refill at Wells or by killing monsters.
");

        Assert.Equal(ItemClass.Charms, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Stone Charm", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);

        Assert.Equal(16, actual.Properties.RequiresLevel);
        Assert.Equal(51, actual.Properties.ItemLevel);

        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Duration (Charm)", 21);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Charges per use", -19);
        Assert.False(actual.Modifiers[1].MatchedFuzzily);
    }
}
