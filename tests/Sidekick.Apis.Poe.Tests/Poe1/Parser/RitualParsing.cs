using Sidekick.Apis.Poe.Trade;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class RitualParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void RitualSplinter()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Ritual Splinter
--------
Stack Size: 17/100
--------
Combine 100 Splinters to create a Ritual Vessel.
Shift click to unstack.
--------
Note: ~price 1 alch
");

        Assert.Equal("currency", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Currency, actual.Header.Rarity);
        Assert.Equal(Category.Currency, actual.Header.Category);
        Assert.Equal("Ritual Splinter", actual.Header.ApiType);
    }

    [Fact]
    public void RitualVessel()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Ritual Vessel
--------
Stack Size: 1/10
--------
Stores the monsters slain for the first time from a completed Ritual Altar for future use
--------
Right-click this item then left-click a Ritual Altar to store the monsters from the completed Ritual in this item. Cannot be used on a Ritual in a map opened with a Blood-Filled Vessel.
--------
Note: ~price 8 chaos
");

        Assert.Equal("currency", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Currency, actual.Header.Rarity);
        Assert.Equal(Category.Currency, actual.Header.Category);
        Assert.Equal("Ritual Vessel", actual.Header.ApiType);
    }

    [Fact]
    public void Corpse()
    {
        var actual = parser.ParseItem(@"Item Class: Corpses
Rarity: Currency
Perfect Needle Horror
--------
Corpse Level: 83
Monster Category: Demon
--------
Item Level: 83
--------
Throws Physical Projectiles (implicit)
100% increased Impale Effect (implicit)
Owner gains 10% increased Impale Effect (implicit)
--------
Right click this item to create this corpse.
--------
Note: ~price 3 chaos
");

        Assert.Equal("corpse", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Currency, actual.Header.Rarity);
        Assert.Equal(Category.Corpse, actual.Header.Category);
        Assert.Equal("Perfect Needle Horror", actual.Header.ApiType);
    }
}
