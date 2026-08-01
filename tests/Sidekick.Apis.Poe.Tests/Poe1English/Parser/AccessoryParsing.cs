using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.ItemClasses;
using Sidekick.Data.Items;
using Sidekick.Data.Stats;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class AccessoryParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseBroodCircle()
    {
        var actual = parser.ParseItem(@"Item Class: Rings
Rarity: Rare
Brood Circle
Ruby Ring
--------
Requirements:
Level: 36
--------
Item Level: 76
--------
Anger has 18% increased Aura Effect (implicit)
--------
+16 to all Attributes
+31 to Intelligence
Adds 8 to 13 Physical Damage to Attacks
31% increased Mana Regeneration Rate
--------
Corrupted
");

        Assert.Equal(ItemClass.Ring, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Ruby Ring", actual.Definition.TradeItem?.Type);

        Assert.Equal(76, actual.Properties.ItemLevel);
        Assert.False(actual.Properties.Unidentified);
        Assert.True(actual.Properties.Corrupted);

        fixture.AssertHasStat(actual, StatCategory.Implicit, "Anger has #% increased Aura Effect", 18);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "+# to all Attributes", 16);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "+# to Intelligence", 31);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Adds # to # Physical Damage to Attacks", 8, 13);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Mana Regeneration Rate", 31);
    }

    [Fact]
    public void ParseBerekGrip()
    {
        var actual = parser.ParseItem(@"Item Class: Rings
Rarity: Unique
Berek's Grip
Two-Stone Ring
--------
Requirements:
Level: 20
--------
Item Level: 84
--------
+13% to Cold and Lightning Resistances (implicit)
--------
28% increased Cold Damage
Adds 1 to 67 Lightning Damage to Spells and Attacks
+30 to maximum Life
1% of Damage Leeched as Life against Shocked Enemies
1% of Damage Leeched as Energy Shield against Frozen Enemies
--------
""Berek hid from Storm's lightning wrath
In the embrace of oblivious Frost
Repelled by ice, blinded by blizzards
Storm raged in vain
While Berek slept.""
- Berek and the Untamed
");

        Assert.Equal(ItemClass.Ring, actual.ItemClass.Type);
        Assert.False(actual.Properties.Unidentified);
    }

    [Fact]
    public void ParsePrecursorEmblemRuby()
    {
        var actual = parser.ParseItem(@"Item Class: Rings
Rarity: Unique
Precursor's Emblem
Ruby Ring
--------
Requirements:
Level: 49
--------
Item Level: 85
--------
+23% to Fire Resistance (implicit)
--------
+20 to Strength
5% increased maximum Energy Shield
5% increased maximum Life
Regenerate 0.3% of Life per second per Endurance Charge
You cannot be Stunned while at maximum Endurance Charges
1% increased Movement Speed per Endurance Charge
--------
History teaches humility.
--------
Note: ~b/o 2 chaos
");

        Assert.Equal(ItemClass.Ring, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Precursor's Emblem", actual.Definition.TradeItem?.Name);
        Assert.Equal("Ruby Ring", actual.Definition.TradeItem?.Type);
    }

    [Fact]
    public void ParsePrecursorEmblemSapphire()
    {
        var actual = parser.ParseItem(@"Item Class: Rings
Rarity: Unique
Precursor's Emblem
Sapphire Ring
--------
Requirements:
Level: 49
--------
Item Level: 85
--------
+29% to Cold Resistance (implicit)
--------
+20 to Dexterity
8% increased Evasion Rating per Frenzy Charge
5% increased maximum Energy Shield
5% increased maximum Life
20% increased Frenzy Charge Duration
5% increased Damage per Frenzy Charge
--------
History teaches humility.
--------
Note: ~b/o 20 chaos
");

        Assert.Equal(ItemClass.Ring, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Precursor's Emblem", actual.Definition.TradeItem?.Name);
        Assert.Equal("Sapphire Ring", actual.Definition.TradeItem?.Type);
    }

    [Fact]
    public void KalandraTouch()
    {
        var actual = parser.ParseItem(@"Item Class: Rings
Rarity: Unique
Kalandra's Touch
Ring
--------
Item Level: 85
--------
Reflects opposite Ring
--------
On one hand, you have a choice.
On the other, you have its twin.
--------
Mirrored
");

        Assert.Equal(ItemClass.Ring, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Kalandra's Touch", actual.Definition.TradeItem?.Name);
        Assert.Equal("Ring", actual.Definition.TradeItem?.Type);
        Assert.True(actual.Properties.Mirrored);
    }

    [Fact]
    public void PearlescentAmulet()
    {
        var actual = parser.ParseItem(@"Item Class: Amulets
Rarity: Rare
Maelström Charm
Pearlescent Amulet
--------
Requirements:
Level: 35
--------
Item Level: 72
--------
{ Implicit Modifier — Elemental, Resistance }
+10(8-10)% to all Elemental Resistances
--------
{ Prefix Modifier ""Annealed"" (Tier: 4) — Damage, Physical, Attack }
Adds 8(6-9) to 15(13-15) Physical Damage to Attacks
{ Prefix Modifier ""Magpie's"" (Tier: 4) — Drop }
9(8-12)% increased Rarity of Items found
{ Suffix Modifier ""of the Brute"" (Tier: 9) — Attribute }
+8(8-12) to Strength
{ Suffix Modifier ""of the Prism"" (Tier: 5) — Elemental, Resistance }
+6(6-8)% to all Elemental Resistances
");

        Assert.Equal(ItemClass.Amulet, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal("Pearlescent Amulet", actual.Definition.TradeItem?.Type);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "Adds # to # Physical Damage to Attacks", 8, 15);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Rarity of Items found", 9);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "+# to Strength", 8);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "+#% to all Elemental Resistances", 6);
    }

    [Fact]
    public void VictoryEye()
    {
        var actual = parser.ParseItem(@"Item Class: Rings
Rarity: Rare
Victory Eye
Paua Ring
--------
Requirements:
Level: 40
--------
Item Level: 85
--------
{ Corruption Implicit Modifier — Mana, Aura }
Hatred has 22(20-30)% increased Mana Reservation Efficiency
--------
{ Prefix Modifier ""Annealed"" (Tier: 1) — Damage, Physical, Attack }
Adds 9(6-9) to 15(13-15) Physical Damage to Attacks
{ Prefix Modifier ""Arcing"" (Tier: 4) — Damage, Elemental, Lightning, Attack }
Adds 2(1-4) to 41(40-43) Lightning Damage to Attacks
{ Suffix Modifier ""of Talent"" (Tier: 3) — Caster, Speed }
7(5-8)% increased Cast Speed
{ Suffix Modifier ""of the Walrus"" (Tier: 4) — Elemental, Cold, Resistance }
+35(30-35)% to Cold Resistance
--------
Corrupted
");

        Assert.Equal(ItemClass.Ring, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal("Paua Ring", actual.Definition.TradeItem?.Type);

        fixture.AssertHasStat(actual, StatCategory.Implicit, "Hatred has #% increased Mana Reservation Efficiency", 22);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Adds # to # Physical Damage to Attacks", 9, 15);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Adds # to # Lightning Damage to Attacks", 2, 41);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Cast Speed", 7);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "+#% to Cold Resistance", 35);
    }

    [Fact]
    public void UrsaTalisman()
    {
        var actual = parser.ParseItem(@"Item Class: Amulets
Rarity: Rare
Miracle Gorget
Ursa Talisman
--------
Requirements:
Level: 61
--------
Item Level: 85
--------
30% increased Effect of your Marks (enchant)
--------
{ Prefix Modifier ""Dragon's"" (Tier: 2) — Drop }
19(19-24)% increased Rarity of Items found
{ Prefix Modifier ""Unassailable"" (Tier: 1) — Defences, Energy Shield }
21(20-22)% increased maximum Energy Shield
{ Suffix Modifier ""of the Storm"" (Tier: 6) — Elemental, Lightning, Resistance }
+18(18-23)% to Lightning Resistance
{ Suffix Modifier ""of the Goliath"" (Tier: 4) — Attribute }
+35(33-37) to Strength
--------
""With the Hag's daughters easing the climate,
an age of plenty and bounty began. Lysanda
revived the Phaaryl tradition of the hunt,
so the people never forgot their roots.""
");

        Assert.Equal(ItemClass.Amulet, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal("Ursa Talisman", actual.Definition.TradeItem?.Type);

        fixture.AssertHasStat(actual, StatCategory.Enchant, "#% increased Effect of your Marks", 30);
    }

    [Fact]
    public void ReplicaDragonfangsFlight()
    {
        var actual = parser.ParseItem(@"Item Class: Amulets
Rarity: Unique
Replica Dragonfang's Flight
Onyx Amulet
--------
Requirements:
Level: 56
--------
Item Level: 77
--------
+12 to all Attributes (implicit)
(Attributes are Strength, Dexterity, and Intelligence) (implicit)
--------
+3 to Level of all Defiance Banner(Fireball-Mana-Infused Staff) Gems
+5% to all Elemental Resistances
5% increased Reservation Efficiency of Skills
Items and Gems have 6% reduced Attribute Requirements
(Attributes are Strength, Dexterity, and Intelligence)
--------
""Did we make this? Why do we have no record of it?
We were warned that there would be consequences...""
- Administrator Qotra
");

        Assert.Equal(ItemClass.Amulet, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Replica Dragonfang's Flight", actual.Definition.TradeItem?.Name);
        Assert.Equal("Onyx Amulet", actual.Definition.TradeItem?.Type);
    }
}
