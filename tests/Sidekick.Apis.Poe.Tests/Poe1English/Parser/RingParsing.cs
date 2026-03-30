using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Items;
using Sidekick.Data.Stats;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class RingParsing(Poe1EnglishFixture fixture)
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

        Assert.Equal(ItemClass.Ring, actual.ItemClass);
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

        Assert.Equal(ItemClass.Ring, actual.ItemClass);
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

        Assert.Equal(ItemClass.Ring, actual.ItemClass);
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

        Assert.Equal(ItemClass.Ring, actual.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Precursor's Emblem", actual.Definition.TradeItem?.Name);
        Assert.Equal("Sapphire Ring", actual.Definition.TradeItem?.Type);
    }
}
