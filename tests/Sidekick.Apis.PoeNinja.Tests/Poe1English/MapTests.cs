using System.Threading.Tasks;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.PoeNinja.Tests.Poe1English;

[Collection(Collections.NinjaTestCollection)]
public class MapTests(NinjaTestFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public async Task Tier16()
    {
        var item = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Hidden Realm
Map (Tier 16)
--------
Item Quantity: +116% (augmented)
Item Rarity: +68% (augmented)
Monster Pack Size: +44% (augmented)
--------
Item Level: 84
--------
Monster Level: 83
--------
Monsters have 363% increased Critical Strike Chance
+45% to Monster Critical Strike Multiplier
Monsters fire 2 additional Projectiles
Monsters deal 104% extra Physical Damage as Cold
Monsters deal 100% extra Physical Damage as Lightning
+25% Monster Chaos Resistance
+40% Monster Elemental Resistances
Monsters Blind on Hit
Players have 60% reduced effect of Non-Curse Auras from Skills
Players have 25% less Area of Effect
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
--------
Corrupted
");

        var results = await fixture.NinjaStashProvider.GetInfo(item);
        Assert.Single(results);

        var result = results[0];
        Assert.Equal("map-tier-16-t0-gen-24", result.DetailsId);
    }

    [Fact]
    public async Task Nightmare()
    {
        var item = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Cruel Stone
Nightmare Map
--------
Item Quantity: +110% (augmented)
Item Rarity: +111% (augmented)
Monster Pack Size: +57% (augmented)
More Maps: +35% (augmented)
More Currency: +47% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
Players are Cursed with Vulnerability
126% more Monster Life
33% increased Monster Damage
Monsters cannot be Stunned
Monsters have +1 to Maximum Frenzy Charges
Monsters gain a Frenzy Charge on Hit
Monsters cannot be Taunted
Monsters' Action Speed cannot be modified to below Base Value
Monsters' Movement Speed cannot be modified to below Base Value
Players cannot Regenerate Life, Mana or Energy Shield
The Maven interferes with Players
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
--------
Corrupted
--------
Modifiable only with Chaos Orbs, Vaal Orbs, Delirium Orbs and Chisels
");

        var results = await fixture.NinjaStashProvider.GetInfo(item);
        Assert.Single(results);

        var result = results[0];
        Assert.Equal("nightmare-map-t0-gen-24", result.DetailsId);
    }

    [Fact]
    public async Task BlightedMapTier3()
    {
        var item = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Blighted Map (Tier 3)
--------
Map Area: Tropical Island
--------
Item Level: 79
--------
Monster Level: 70
--------
Area is infested with Fungal Growths (implicit)
Map's Item Quantity Modifiers also affect Blight Chest count at 25% value (implicit)
Can be Anointed up to 3 times (implicit)
Natural inhabitants of this area have been removed (implicit)
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
--------
Note: ~b/o 1 chaos
");

        var results = await fixture.NinjaStashProvider.GetInfo(item);
        Assert.Single(results);

        var result = results[0];
        Assert.Equal("blighted-map-tier-3-t0-gen-24", result.DetailsId);
    }

    [Fact]
    public async Task BlightRavagedMap()
    {
        var item = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Blight-ravaged Map (Tier 16)
--------
Map Area: Relic Chambers
--------
Item Level: 83
--------
Monster Level: 85
--------
Monster Level: 85 (implicit)
200% more Monster Life (implicit)
20% increased Monster Movement Speed (implicit)
Area is infested with Fungal Growths (implicit)
Map's Item Quantity Modifiers also affect Blight Chest count at 50% value (implicit)
Can be Anointed up to 9 times (implicit)
Natural inhabitants of this area have been removed (implicit)
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        var results = await fixture.NinjaStashProvider.GetInfo(item);
        Assert.Single(results);

        var result = results[0];
        Assert.Equal("blight-ravaged-map-tier-16-t0-gen-24", result.DetailsId);
    }
}
