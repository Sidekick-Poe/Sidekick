using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.Items;
using Sidekick.Data.Stats;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class HelmetParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseBlightGuardian()
    {
        var actual = parser.ParseItem(@"Item Class: Helmets
Rarity: Rare
Blight Guardian
Hunter Hood
--------
Evasion Rating: 231 (augmented)
--------
Requirements:
Level: 64
Dex: 87
--------
Sockets: G
--------
Item Level: 80
--------
Adds 28 to 51 Fire Damage to Spells
+28 to Evasion Rating
+47 to maximum Life
11% increased Rarity of Items found
+29% to Cold Resistance
You have Shocking Conflux for 3 seconds every 8 seconds
--------
Hunter Item
");

        Assert.Equal(ItemClass.Helmet, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Hunter Hood", actual.Definition.Type);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "You have # Conflux for 3 seconds every 8 seconds", "Shocking");
    }

    [Fact]
    public void ParseStarkonjaHead()
    {
        var actual = parser.ParseItem(@"Item Class: Helmets
Rarity: Unique
Starkonja's Head
Silken Hood
--------
Evasion Rating: 793 (augmented)
--------
Requirements:
Level: 60
Dex: 138
--------
Sockets: G
--------
Item Level: 63
--------
+53 to Dexterity
50% reduced Damage when on Low Life
10% increased Attack Speed
25% increased Global Critical Strike Chance
124% increased Evasion Rating
+80 to maximum Life
150% increased Global Evasion Rating when on Low Life
--------
There was no hero made out of Starkonja's death,
but merely a long sleep made eternal.
--------
Note: ~price 1 chaos
");

        Assert.Equal(ItemClass.Helmet, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Starkonja's Head", actual.Definition.Name);
        Assert.Equal("Silken Hood", actual.Definition.Type);

        Assert.False(actual.Properties.Unidentified);
        Assert.Equal(63, actual.Properties.ItemLevel);
        Assert.Equal(793, actual.Properties.EvasionRating);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "+# to Dexterity", 53);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Damage when on Low Life", -50);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Attack Speed", 10);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Global Critical Strike Chance", 25);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Evasion Rating", 124);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "+# to maximum Life", 80);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Global Evasion Rating when on Low Life", 150);
    }

    [Fact]
    public void ParseTheDarkMonarch()
    {
        var actual = parser.ParseItem(@"Item Class: Helmets
Rarity: Unique
The Dark Monarch
Lich's Circlet
--------
Energy Shield: 217 (augmented)
--------
Requirements:
Level: 84
Int: 224
--------
Sockets: B 
--------
Item Level: 85
--------
+87 to maximum Energy Shield
+1 to Level of all Minion Skill Gems
+35% to Chaos Resistance
50% reduced Light Radius
Maximum number of Summoned Skeletons is Doubled
Cannot have Minions other than Summoned Skeletons
--------
""Hate? You speak to me of hate? You have no idea what your persecution inflicts.
How it chokes the heart. Withers the soul. Judge me, and you judge yourself.""
- Saresh, last words, to Sekhema Orbala
");

        Assert.Equal(ItemClass.Helmet, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("The Dark Monarch", actual.Definition.Name);
        Assert.Equal("Lich's Circlet", actual.Definition.Type);

        Assert.False(actual.Properties.Unidentified);
        Assert.Equal(85, actual.Properties.ItemLevel);
        Assert.Equal(217, actual.Properties.EnergyShield);

        var stat = fixture.AssertHasStat(actual, StatCategory.Explicit, "Maximum number of Summoned Skeletons is Doubled\nCannot have Minions other than Summoned Skeletons");
        Assert.NotNull(stat);
        Assert.Single(stat.Definitions.SelectMany(x => x.TradeStats));
    }
}
