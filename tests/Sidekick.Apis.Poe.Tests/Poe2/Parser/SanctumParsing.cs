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

        Assert.Equal(Category.Sanctum, actual.Header.Category);
        Assert.Equal(Rarity.Magic, actual.Header.Rarity);
        Assert.Equal("sanctum.relic", actual.Header.ApiItemCategory);
        Assert.Equal("Urn Relic", actual.Header.ApiType);
        Assert.Null(actual.Header.ApiName);

        actual.AssertHasModifier(ModifierCategory.Sanctum, "Fountains have #% chance to grant double Sacred Water", 6);
        actual.AssertHasModifier(ModifierCategory.Sanctum, "#% increased Honour restored", 9);
    }

    [Fact]
    public void ParseDodgeRelic()
    {
        var actual = parser.ParseItem(
            @"Item Class: Relics
Rarity: Magic
Layered Urn Relic of Eluding
--------
Item Level: 80
--------
35% increased Defences
+0.7 metres to Dodge Roll distance
--------
Place this item on the Relic Altar at the start of the Trial of the Sekhemas");

        Assert.Equal(Category.Sanctum, actual.Header.Category);
        Assert.Equal(Rarity.Magic, actual.Header.Rarity);
        Assert.Equal("sanctum.relic", actual.Header.ApiItemCategory);
        Assert.Equal("Urn Relic", actual.Header.ApiType);
        Assert.Null(actual.Header.ApiName);
        Assert.Equal(80, actual.Properties.ItemLevel);

        actual.AssertHasModifier(ModifierCategory.Sanctum, "#% increased Defences", 35);
        actual.AssertHasModifier(ModifierCategory.Sanctum, "# metre to Dodge Roll distance", 0.7);
    }
}
