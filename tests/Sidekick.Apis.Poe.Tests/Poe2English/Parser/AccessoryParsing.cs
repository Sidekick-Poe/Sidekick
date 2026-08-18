using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Stats;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe2English.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class AccessoryParsing(Poe2EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void GloomLash()
    {
        var actual = parser.ParseItem(@"Item Class: Belts
Rarity: Rare
Gloom Lash
Utility Belt
--------
Requires: Level 59
--------
Item Level: 81
--------
{ Implicit Modifier }
20% of Flask Recovery applied Instantly
{ Implicit Modifier — Charm }
Has 1(1-3) Charm Slot
--------
{ Desecrated Prefix Modifier ""Kurgal's"" (Tier: 1) — Mana, Armour }
Gain 9(6-12)% of Maximum Mana as Armour
{ Prefix Modifier ""Jagged"" (Tier: 1) — Damage, Physical }
136(101-151) to 154(152-220) Physical Thorns damage
{ Crafted Prefix Modifier ""Verisium"" }
22(20-30)% increased Explicit Resistance Modifier magnitudes — Unscalable Value
{ Suffix Modifier ""of the Maelstrom"" (Tier: 3) — Elemental, Lightning, Resistance — 22% Increased }
+33(31-35)% to Lightning Resistance
{ Suffix Modifier ""of the Furnace"" (Tier: 4) — Elemental, Fire, Resistance — 22% Increased }
+29(26-30)% to Fire Resistance
{ Suffix Modifier ""of Eviction"" (Tier: 4) — Chaos, Resistance — 22% Increased }
+13(12-15)% to Chaos Resistance
--------
Note: ~b/o 2 divine
");

        Assert.Equal(ItemClass.Belt, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Utility Belt", actual.TradeItem?.Type);

        Assert.Equal(81, actual.Properties.ItemLevel);
        Assert.False(actual.Properties.Split);
        Assert.False(actual.Properties.Fractured);
        Assert.True(actual.Properties.Desecrated);

        fixture.AssertHasStat(actual, StatCategory.Implicit, "#% of Flask Recovery applied Instantly", 20);
        fixture.AssertHasStat(actual, StatCategory.Implicit, "Has # Charm Slot", 1);
        fixture.AssertHasStat(actual, StatCategory.Desecrated, "Gain #% of Maximum Mana as Armour", 9);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "# to # Physical Thorns damage", 136, 154);
        fixture.AssertHasStat(actual, StatCategory.Crafted, "#% increased Explicit Resistance Modifier magnitudes", 22);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% to Lightning Resistance", 33);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% to Fire Resistance", 29);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% to Chaos Resistance", 13);
    }
}
