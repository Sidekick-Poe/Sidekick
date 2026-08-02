using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.ItemClasses;
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

        Assert.Equal(ItemClass.Helmet, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Hunter Hood", actual.Definition.TradeItem?.Type);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "You have Shocking Conflux for 3 seconds every 8 seconds");
        fixture.AssertDoesNotHaveStat(actual, StatCategory.Explicit, "You have Chilling Conflux for 3 seconds every 8 seconds");
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

        Assert.Equal(ItemClass.Helmet, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Starkonja's Head", actual.Definition.TradeItem?.Name);
        Assert.Equal("Silken Hood", actual.Definition.TradeItem?.Type);

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
Energy Shield: 189 (augmented)
--------
Requirements:
Level: 84
Int: 224 (unmet)
--------
Sockets: W 
--------
Item Level: 85
--------
{ Unique Modifier — Defences, Energy Shield }
+60(50-100) to maximum Energy Shield
{ Unique Modifier — Minion, Gem }
+1 to Level of all Minion Skill Gems
{ Unique Modifier — Chaos, Resistance }
+36(27-37)% to Chaos Resistance
{ Unique Modifier }
50% reduced Light Radius
{ Unique Modifier }
Maximum number of Raised Spiders (Animated Weapons-Holy Armaments) is Doubled
Cannot have Minions other than Raised Spiders (Animated Weapons-Holy Armaments)
--------
""Hate? You speak to me of hate? You have no idea what your persecution inflicts.
How it chokes the heart. Withers the soul. Judge me, and you judge yourself.""
- Saresh, last words, to Sekhema Orbala
");

        Assert.Equal(ItemClass.Helmet, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("The Dark Monarch", actual.Definition.TradeItem?.Name);
        Assert.Equal("Lich's Circlet", actual.Definition.TradeItem?.Type);

        Assert.False(actual.Properties.Unidentified);
        Assert.Equal(85, actual.Properties.ItemLevel);
        Assert.Equal(189, actual.Properties.EnergyShield);

        var stat = fixture.AssertHasStat(actual, StatCategory.Explicit, "Maximum number of Raised Spiders is Doubled\nCannot have Minions other than Raised Spiders");
        Assert.NotNull(stat);
        Assert.Single(stat.Definitions
                          .Where(x => x.TradeIds != null)
                          .SelectMany(x => x.TradeIds!));
        Assert.False(stat.HasValues);
    }

    [Fact]
    public void VestigialSecutorHelm()
    {
        var actual = parser.ParseItem(@"Item Class: Helmets
Rarity: Rare
Havoc Ward
Vestigial Secutor Helm
--------
Armour: 164 (augmented)
Evasion Rating: 164 (augmented)
--------
Requirements:
Level: 60
Str: 42
Dex: 42
--------
Sockets: G-W-W 
--------
Item Level: 85
--------
{ Vestigial Implicit Modifier — Elemental, Fire, Lightning, Ailment }
Your Fire Damage can Shock but not Ignite
(Shock increases Damage taken by up to 50%, depending on the amount of Lightning Damage in the hit, for 2 seconds)
--------
{ Prefix Modifier ""Dragon's"" (Tier: 1) — Drop }
23(19-24)% increased Rarity of Items found
{ Prefix Modifier ""Brawler's"" (Tier: 6) — Defences, Armour, Evasion }
35(27-42)% increased Armour and Evasion
{ Prefix Modifier ""Rhino's"" (Tier: 3) — Defences, Armour, Evasion }
31(27-32)% increased Armour and Evasion
12(12-13)% increased Stun and Block Recovery
{ Suffix Modifier ""of Excavation"" (Tier: 1) — Drop }
23(21-26)% increased Rarity of Items found
{ Suffix Modifier ""of the Sniper"" (Tier: 4) — Attack }
+231(166-250) to Accuracy Rating
{ Suffix Modifier ""of the Thunderhead"" (Tier: 5) — Elemental, Lightning, Resistance }
+24(24-29)% to Lightning Resistance
--------
Corrupted
");

        Assert.Equal(ItemClass.Helmet, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal("Secutor Helm", actual.Definition.TradeItem?.Type);

        Assert.True(actual.Properties.Corrupted);
        Assert.Equal(85, actual.Properties.ItemLevel);
        Assert.Equal(164, actual.Properties.Armour);
        Assert.Equal(164, actual.Properties.EvasionRating);
        Assert.Equal(0, actual.Properties.EnergyShield);

        fixture.AssertHasStat(actual, StatCategory.Implicit, "Your Fire Damage can Shock but not Ignite");
    }
}
