using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.Items;
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

        Assert.Equal(ItemClass.Ring, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Ruby Ring", actual.ApiInformation.Type);

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

        Assert.Equal(ItemClass.Ring, actual.Properties.ItemClass);
        Assert.False(actual.Properties.Unidentified);
    }
}
