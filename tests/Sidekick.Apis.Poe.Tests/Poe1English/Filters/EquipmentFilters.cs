using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Filters;

[Collection(Collections.Poe1EnglishFixture)]
public class EquipmentFilters(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public async Task UniqueBodyArmour()
    {
        var item = parser.ParseItem(@"Item Class: Body Armours
Rarity: Unique
Carcass Jack
Varnished Coat
--------
Quality: +20% (augmented)
Evasion Rating: 960 (augmented)
Energy Shield: 186 (augmented)
--------
Requirements:
Level: 70
Str: 68
Dex: 96
Int: 111
--------
Sockets: B-B-R-B-B-B
--------
Item Level: 81
--------
128% increased Evasion and Energy Shield
+55 to maximum Life
+12% to all Elemental Resistances
44% increased Area of Effect
47% increased Area Damage
Extra gore
--------
""...The discomfort shown by the others is amusing, but none
can deny that my work has made quite the splash...""
- Maligaro's Journal
");

        var filters = await fixture.GetPropertyFilters(item);
        Assert.Equal(13, filters.Count);
        var index = -1;
        Assert.IsType<UniqueRarityFilter>(filters[++index]);
        Assert.IsType<QualityFilter>(filters[++index]);
        Assert.IsType<EvasionRatingFilter>(filters[++index]);
        Assert.IsType<EnergyShieldFilter>(filters[++index]);
        Assert.IsType<ItemLevelFilter>(filters[++index]);
        Assert.IsType<SocketFilter>(filters[++index]);
        Assert.IsType<RequiresLevelFilter>(filters[++index]);
        Assert.IsType<RequiresStrengthFilter>(filters[++index]);
        Assert.IsType<RequiresDexterityFilter>(filters[++index]);
        Assert.IsType<RequiresIntelligenceFilter>(filters[++index]);
        Assert.IsType<CorruptedFilter>(filters[++index]);
        Assert.IsType<FoulbornFilter>(filters[++index]);
        Assert.IsType<UnidentifiedFilter>(filters[++index]);
    }
}
