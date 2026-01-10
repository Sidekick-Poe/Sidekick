using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class SanctumParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseFloor()
    {
        var actual = parser.ParseItem(@"Item Class: Sanctum Research
Rarity: Currency
Forbidden Tome
--------
Area Level: 83
--------
Item Level: 84
--------
Mundus noster cecidit. Daemones ubique sunt. Librum
hunc in sacrarium conicio, ut forte alius viam inveniat...
--------
Take this item to the Relic Altar in the Forbidden Sanctum to enter.
");

        Assert.Equal(ItemClass.SanctumResearch, actual.Properties.ItemClass);
        Assert.Equal("Forbidden Tome", actual.ApiInformation.Type);
        Assert.Equal(83, actual.Properties.AreaLevel);
        Assert.Equal(84, actual.Properties.ItemLevel);
    }

    [Fact]
    public void ParseRelic()
    {
        var actual = parser.ParseItem(@"Item Class: Relics
Rarity: Magic
Steadfast Urn Relic of Achievement
--------
Item Level: 80
--------
Gain 20 Resolve when you kill a Boss
+5 to Maximum Resolve
--------
Place this item on the Relic Altar at the start of each Sanctum run
--------
Unmodifiable
");

        Assert.Equal(ItemClass.SanctumRelic, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Urn Relic", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);
        Assert.Equal(80, actual.Properties.ItemLevel);

        actual.AssertHasStat(StatCategory.Sanctum, "Gain # Resolve when you kill a Boss", 20);
        actual.AssertHasStat(StatCategory.Sanctum, "+# to Maximum Resolve", 5);
    }
}
