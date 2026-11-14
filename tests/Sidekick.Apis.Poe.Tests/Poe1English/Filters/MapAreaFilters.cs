using Sidekick.Game.Parser;
using Sidekick.Game.Parser.Properties.Definitions;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Filters;

[Collection(Collections.Poe1EnglishFixture)]
public class MapAreaFilters(Poe1EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public async Task ScryingOrbHasCheckedReadonlyMapAreaFilter()
    {
        var item = parser.ParseItem(ScryingOrb("Volcano"));

        var filters = await fixture.GetPropertyFilters(item);
        var filter = Assert.Single(filters.OfType<MapAreaFilter>());

        Assert.Equal("Volcano", filter.Value);
        Assert.True(filter.Checked);
        Assert.True(filter.Readonly);
    }

    [Fact]
    public async Task ScryingOrbWithoutAreaTradeItemHasNoMapAreaFilter()
    {
        var item = parser.ParseItem(ScryingOrb("Necropolis"));

        var filters = await fixture.GetPropertyFilters(item);
        Assert.Empty(filters.OfType<MapAreaFilter>());
    }

    [Fact]
    public async Task MapHasNoMapAreaFilter()
    {
        var item = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Blighted Map (Tier 13)
--------
Map Area: Necropolis
--------
Item Level: 83
--------
Monster Level: 80
--------
Area is infested with Fungal Growths (implicit)
Map's Item Quantity Modifiers also affect Blight Chest count at 25% value (implicit)
Can be Anointed up to 3 times (implicit)
Natural inhabitants of this area have been removed (implicit)
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal("Necropolis", item.Properties.MapArea);
        var filters = await fixture.GetPropertyFilters(item);
        Assert.Empty(filters.OfType<MapAreaFilter>());
    }

    private static string ScryingOrb(string mapArea) => $@"Item Class: Stackable Currency
Rarity: Currency
Scrying Orb
--------
Map Area: {mapArea}
--------
Scries a Map on your Atlas
--------
Right click on this item then left click a Map on your Atlas.
";
}
