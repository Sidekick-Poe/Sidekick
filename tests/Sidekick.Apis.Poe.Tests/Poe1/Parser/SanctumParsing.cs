using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class SanctumParsing(ParserFixture fixture)
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

        Assert.Equal(Category.Sanctum, actual.Header.Category);
        Assert.Equal("sanctum.research", actual.Header.ApiItemCategory);
        Assert.Equal("Forbidden Tome", actual.Header.ApiType);
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

        Assert.Equal(Category.Sanctum, actual.Header.Category);
        Assert.Equal(Rarity.Magic, actual.Header.Rarity);
        Assert.Equal("sanctum.relic", actual.Header.ApiItemCategory);
        Assert.Equal("Urn Relic", actual.Header.ApiType);
        Assert.Null(actual.Header.ApiName);
        Assert.Equal(80, actual.Properties.ItemLevel);

        actual.AssertHasModifier(ModifierCategory.Sanctum, "Gain # Resolve when you kill a Boss", 20);
        actual.AssertHasModifier(ModifierCategory.Sanctum, "+# to Maximum Resolve", 5);
    }
}
