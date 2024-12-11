using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe2.Parser;

[Collection(Collections.Poe2Parser)]
public class SanctumParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseRelic()
    {
        var actual = parser.ParseItem(
            @"Item Class: Relics
Rarity: Magic
Revitalising Urn Relic of Flowing
--------
Item Level: 22
--------
Fountains have 6% chance to grant double Sacred Water
9% increased Honour restored
--------
Place this item on the Relic Altar at the start of the Trial of the Sekhemas
");

        Assert.Equal(Category.Sanctum, actual.Metadata.Category);
        Assert.Equal(Rarity.Magic, actual.Metadata.Rarity);
        Assert.Equal(Class.SanctumResearch, actual.Header.Class);
        Assert.Equal("Urn Relic", actual.Metadata.Type);
        Assert.Equal("Urn Relic", actual.Metadata.Name);

        actual.AssertHasModifier(ModifierCategory.Explicit, "Fountains have #% chance to grant double Sacred Water", 6);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Honour restored", 9);
        // Ass
    }
}
