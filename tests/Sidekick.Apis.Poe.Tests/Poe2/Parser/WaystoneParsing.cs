using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe2.Parser;

[Collection(Collections.Poe2Parser)]
public class WaystoneParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseWaystoneProperties()
    {
        var actual = parser.ParseItem(
            @"Item Class: Waystones
Rarity: Rare
Forsaken Bearings
Waystone (Tier 1)
--------
Waystone Tier: 1
Revives Available: 2 (augmented)
Monster Pack Size: +12% (augmented)
Magic Monsters: +30% (augmented)
Rare Monsters: +28% (augmented)
Item Rarity: +18% (augmented)
Waystone Drop Chance: +70% (augmented)
--------
Item Level: 66
--------
19% increased Monster Damage
14% increased Monster Movement Speed
14% increased Monster Attack Speed
11% increased Monster Cast Speed
Monsters have 30% increased Ailment Threshold
Monsters have 75% increased Freeze Buildup
Monsters inflict 75% increased Flammability Magnitude
Monsters have 75% increased Shock Chance
Monsters have 35% increased Stun Threshold
--------
Can be used in a Map Device, allowing you to enter a Map. Waystones can only be used once.
");

        Assert.Equal(ItemClass.Waystone, actual.Properties.ItemClass);
        Assert.Equal("Waystone (Tier 1)", actual.ApiInformation.Type);
        Assert.Null(actual.ApiInformation.Name);
        Assert.Equal(66, actual.Properties.ItemLevel);

        Assert.Equal(2, actual.Properties.RevivesAvailable);
        Assert.Equal(12, actual.Properties.MonsterPackSize);
        Assert.Equal(30, actual.Properties.MagicMonsters);
        Assert.Equal(28, actual.Properties.RareMonsters);
        Assert.Equal(18, actual.Properties.ItemRarity);
        Assert.Equal(70, actual.Properties.WaystoneDropChance);
    }
}
