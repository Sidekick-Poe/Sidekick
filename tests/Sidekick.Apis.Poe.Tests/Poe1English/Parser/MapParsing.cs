using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.Items;
using Sidekick.Data.Stats;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class MapParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void Tier2()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Magic
Armoured Map (Tier 2)
--------
Item Quantity: +13% (augmented)
Item Rarity: +8% (augmented)
Monster Pack Size: +5% (augmented)
--------
Item Level: 71
--------
Monster Level: 69
--------
+20% Monster Physical Damage Reduction
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.

");

        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Null(actual.Definition.Name);
        Assert.Equal("Map", actual.Definition.Type);
        Assert.Equal(2, actual.Properties.MapTier);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "+#% Monster Physical Damage Reduction", 20);
    }

    [Fact]
    public void OlmecSanctum()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Unique
Olmec's Sanctum
Map (Tier 6)
--------
Item Quantity: +179% (augmented)
--------
Item Level: 73
--------
Monster Level: 73
--------
42% more Monster Life
37% increased Monster Damage
Final Boss drops higher Level Items
--------
They flew, and leapt, and clambered over,
They crawled, and swam, and slithered under.
Still its ancient secrets await unclaimed
And of this hidden temple, only legends remain.
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Olmec's Sanctum", actual.Definition.Name);
        Assert.Equal("Map", actual.Definition.Type);
        Assert.Equal(6, actual.Properties.MapTier);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% more Monster Life", 42);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Monster Damage", 37);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Final Boss drops higher Level Items");
    }
}
