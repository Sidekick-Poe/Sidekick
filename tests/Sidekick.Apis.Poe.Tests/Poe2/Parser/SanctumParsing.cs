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
        Assert.Equal("sanctum.relic", actual.Header.ItemCategory);
        Assert.Equal("Urn Relic", actual.Metadata.Type);
        Assert.Null(actual.Metadata.Name);

        actual.AssertHasModifier(ModifierCategory.Sanctum, "Fountains have #% chance to grant double Sacred Water", 6);
        actual.AssertHasModifier(ModifierCategory.Sanctum, "#% increased Honour restored", 9);
        // Ass
    }
}
