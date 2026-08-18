using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser;
using Sidekick.Game.Parser.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class AllflameParsing(Poe1EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void ParseRareChart()
    {
        var actual = parser.ParseItem(@"Item Class: Chart
Rarity: Rare
Marine Venture
Coral Forest Chart
--------
Undersea Groves
Area Level: 75
Item Quantity: +60% (augmented)
Monster Pack Size: +14% (augmented)
Gold Found: +50% (augmented)
--------
Requirements:
Level: 54
--------
Item Level: 75
--------
{ Implicit Modifier }
Voyage Modifier will be revealed once Charted
--------
Chart Shape: Crossing
--------
{ Prefix Modifier ""Armoured"" (Tier: 3) — Physical }
+12(9-13)% Monster Physical Damage Reduction
{ Prefix Modifier ""Impervious"" (Tier: 1) — Physical, Chaos, Attack, Ailment }
Monsters have a 40% chance to avoid Poison, Impale, and Bleeding
{ Suffix Modifier ""of Frenzy"" (Tier: 2) }
Monsters have 50% chance to gain a Frenzy Charge on Hit
{ Suffix Modifier ""of Deadliness"" (Tier: 3) — Damage, Critical }
Monsters have 192(180-200)% increased Critical Strike Chance
+32(31-40)% to Monster Critical Strike Multiplier
--------
Take this item to Valerie aboard the Sovereign to Chart this area.
");

        Assert.Equal(ItemClass.Chart, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Coral Forest Chart", actual.TradeItem?.Type);
    }
}
