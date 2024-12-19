using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe2.Parser;

[Collection(Collections.Poe2Parser)]
public class FlaskParsing(ParserFixture fixture)
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

        Assert.Equal(Category.Flask, actual.Metadata.Category);
        Assert.Equal(Rarity.Magic, actual.Metadata.Rarity);
        Assert.Equal("flask.life", actual.Header.ItemCategory);
        Assert.Equal("Ultimate Life Flask", actual.Metadata.Type);
        Assert.Null(actual.Metadata.Name);

        Assert.Equal(66, actual.Properties.ItemLevel);

        actual.AssertHasModifier(ModifierCategory.Explicit, "#% of Recovery applied Instantly", 23);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Charges per use", 26);
    }
}
