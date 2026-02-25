using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.Items.Models;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe2English.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class SanctumParsing(Poe2EnglishFixture fixture)
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

        Assert.Equal(ItemClass.SanctumRelic, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Urn Relic", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);

        actual.AssertHasStat(StatCategory.Sanctum, "Fountains have #% chance to grant double Sacred Water", 6);
        actual.AssertHasStat(StatCategory.Sanctum, "#% increased Honour restored", 9);
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

        Assert.Equal(ItemClass.SanctumRelic, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Urn Relic", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);
        Assert.Equal(80, actual.Properties.ItemLevel);

        actual.AssertHasStat(StatCategory.Sanctum, "#% increased Defences", 35);
        actual.AssertHasStat(StatCategory.Sanctum, "# metre to Dodge Roll distance", 0.7);
    }
}
