using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Stats;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe2.Parser;

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

    [Fact]
    public void LumberingTalismanofConsumption()
    {
        var actual = parser.ParseItem(@"Item Class: Talismans
Rarity: Magic
Lumbering Talisman of Consumption
--------
Physical Damage: 71-107
Critical Hit Chance: 5.00%
Attacks per Second: 1.10
--------
Requires: Level 52, 60 Str, 43 Int
--------
Item Level: 77
--------
Gain 19 Mana per enemy killed
");

        Assert.Equal(ItemClass.Talisman, actual.ItemClass.Type);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Equal("Lumbering Talisman", actual.TradeItem?.Type);
        Assert.Null(actual.Name);

        Assert.Equal(77, actual.Properties.ItemLevel);

        Assert.Equal(71, actual.Properties.PhysicalDamage?.Min);
        Assert.Equal(107, actual.Properties.PhysicalDamage?.Max);
        Assert.Equal(5, actual.Properties.CriticalHitChance);
        Assert.Equal(1.1, actual.Properties.AttacksPerSecond);

        Assert.Equal(0, actual.Properties.RequiresDexterity);
        Assert.Equal(43, actual.Properties.RequiresIntelligence);
        Assert.Equal(60, actual.Properties.RequiresStrength);
        Assert.Equal(52, actual.Properties.RequiresLevel);
    }

    [Fact]
    public async Task CorruptedHeadhunter()
    {
        var actual = parser.ParseItem(@"Item Class: Belts
Rarity: Unique
Headhunter
Heavy Belt
--------
Requires: Level 50
--------
Item Level: 82
--------
+25% to Lightning Resistance (enchant)
--------
{ Implicit Modifier }
23(20-30)% increased Stun Threshold
{ Implicit Modifier — Charm }
Has 2(1-3) Charm Slots
--------
{ Unique Modifier — Attribute }
+37(20-40) to Strength
{ Unique Modifier — Attribute }
+25(20-40) to Dexterity
{ Unique Modifier — Life }
+60(40-60) to maximum Life
{ Unique Modifier }
When you kill a Rare monster, you gain its Modifiers for 60 seconds
--------
""A man's soul rules from a cavern of bone, learns and
judges through flesh-born windows. The heart is meat.
The head is where the Man is.""
Lavianga, Advisor to Kaom
--------
Corrupted
--------
Note: ~b/o 119 divine
");

        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.True(actual.Properties.Corrupted);
        Assert.False(actual.Properties.TwiceCorrupted);
        fixture.AssertHasStat(actual, StatCategory.Enchant, "#% to Lightning Resistance", 25);

        var filters = await fixture.GetPropertyFilters(actual);
        Assert.True(Assert.Single(filters.OfType<Sidekick.Game.Parser.Properties.Definitions.CorruptedFilter>()).Checked);
        var twiceCorrupted = Assert.Single(filters.OfType<Sidekick.Game.Parser.Properties.Definitions.TwiceCorruptedFilter>());
        Assert.Null(twiceCorrupted.Checked);
    }

    [Fact]
    public async Task TwiceCorruptedHeadhunter()
    {
        var actual = parser.ParseItem(@"Item Class: Belts
Rarity: Unique
Headhunter
Heavy Belt
--------
Requires: Level 50
--------
Item Level: 82
--------
+25% to Lightning Resistance (enchant)
--------
{ Implicit Modifier }
23(20-30)% increased Stun Threshold
{ Implicit Modifier — Charm }
Has 2(1-3) Charm Slots
--------
{ Unique Modifier — Attribute }
+37(20-40) to Strength
{ Unique Modifier — Attribute }
+25(20-40) to Dexterity
{ Unique Modifier — Life }
+60(40-60) to maximum Life
{ Unique Modifier }
When you kill a Rare monster, you gain its Modifiers for 60 seconds
--------
""A man's soul rules from a cavern of bone, learns and
judges through flesh-born windows. The heart is meat.
The head is where the Man is.""
Lavianga, Advisor to Kaom
--------
Twice Corrupted
--------
Note: ~b/o 119 divine
");

        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.False(actual.Properties.Corrupted);
        Assert.True(actual.Properties.TwiceCorrupted);
        fixture.AssertHasStat(actual, StatCategory.Enchant, "#% to Lightning Resistance", 25);

        var filters = await fixture.GetPropertyFilters(actual);
        var corrupted = Assert.Single(filters.OfType<Sidekick.Game.Parser.Properties.Definitions.CorruptedFilter>());
        Assert.True(corrupted.Checked);
        var twiceCorrupted = Assert.Single(filters.OfType<Sidekick.Game.Parser.Properties.Definitions.TwiceCorruptedFilter>());
        Assert.Null(twiceCorrupted.Checked);
        Assert.Equal("Twice Corrupted", twiceCorrupted.Text);

        var query = new Sidekick.Game.Parser.Trade.Requests.Query();
        corrupted.PrepareTradeRequest(query, actual);
        twiceCorrupted.PrepareTradeRequest(query, actual);
        var miscFilters = query.Filters.GetOrCreateMiscFilters().Filters;
        Assert.Equal("true", miscFilters.Corrupted?.Option);
        Assert.Null(miscFilters.TwiceCorrupted?.Option);
    }
}
