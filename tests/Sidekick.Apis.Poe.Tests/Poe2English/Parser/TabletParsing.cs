using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Stats;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe2English.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class TabletParsing(Poe2EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void ParseRitualTablet()
    {
        var actual = parser.ParseItem(
            @"Item Class: Tablet
Rarity: Rare
Voidtouched Invocation
Ritual Tablet
--------
Item Level: 79
--------
{ Implicit Modifier }
Adds Ritual Altars to a Map
10 uses remaining
--------
{ Prefix Modifier ""Collector's"" (Tier: 1) }
11(8-12)% increased Rarity of Items found in Map
{ Prefix Modifier ""Elevated"" (Tier: 1) }
17(12-18)% increased Experience gain in Map
{ Suffix Modifier ""of the Devoted"" (Tier: 1) }
Map contains an additional Shrine
{ Suffix Modifier ""of Devotion"" (Tier: 1) }
Deferring Favours at Ritual Altars in Map costs 28(30-20)% reduced Tribute
--------
Can be used in a personal Map Device to add modifiers to a Map.
");

        Assert.Equal(ItemClass.Tablet, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Ritual Tablet", actual.TradeItem?.Type);

        Assert.Equal(79, actual.Properties.ItemLevel);

        fixture.AssertHasStat(actual, StatCategory.Pseudo, "# uses remaining (Tablets)", 10);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Rarity of Items found in Map",11);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Experience gain in Map", 17);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Map contains an additional Shrine");
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Deferring Favours at Ritual Altars in Area costs #% increased Tribute", -28);
    }

     [Fact]
     public void ParseRitualTabletWithAdditionalRareChests()
     {
         var actual = parser.ParseItem(
             @"Item Class: Tablet
Rarity: Rare
Transcendental Judgment
Ritual Tablet
--------
Item Level: 79
--------
{ Implicit Modifier }
Adds Ritual Altars to a Map
10 uses remaining
--------
{ Prefix Modifier ""Elevated"" (Tier: 1) }
17(12-18)% increased Experience gain in Map
{ Prefix Modifier ""Treasurer's"" (Tier: 1) }
Map contains 2(2-3) additional Rare Chests
{ Suffix Modifier ""of the Essence"" (Tier: 1) }
Map has 86(70-100)% increased chance to contain Essences
{ Suffix Modifier ""of the Occult"" (Tier: 1) }
Favours Rerolled at Ritual Altars in Map have 6(3-6)% chance to cost no Tribute
--------
Can be used in a personal Map Device to add modifiers to a Map.");

         fixture.AssertHasStat(actual, StatCategory.Explicit, "Map contains an additional Rare Chest", 2);
     }

     [Theory]
     [InlineData("Map contains 2(1-2) additional Essences", "Map contains an additional Essence", 2)]
     [InlineData("Map contains 2(1-2) additional Shrines", "Map contains an additional Shrine", 2)]
     [InlineData("Map contains 2(1-2) additional Azmeri Spirits", "Map contains # additional Azmeri Spirit", 2)]
     [InlineData("Breach Hives in Map have 4(2-5) additional waves of Hiveborn Monsters", "Breach Hives in Map have an additional wave of Hiveborn Monsters", 4)]
     [InlineData("Unstable Breaches in Map spawn 3(1-3) additional Rare Monsters when Stabilised", "Unstable Breaches in Map spawn an additional Rare Monster when Stabilised", 3)]
     [InlineData("Ritual Altars in Map allow rerolling Favours 2(1-3) additional times", "Ritual Altars in Map allow rerolling Favours an additional time", 2)]
     [InlineData("Slaying Rare Monsters in Map pauses the Delirium Mirror Timer for 4(2-5) seconds", "Slaying Rare Monsters in Map pauses the Delirium Mirror Timer for 1 second", 4)]
     [InlineData("Expeditions contain 2(1-2) Additional Bosses encased in ice in Map", "Expeditions contain 1 Additional Boss encased in ice in Map", 2)]
     [InlineData("Expeditions contain 2(1-2) Vaal Relics in Map", "Expeditions contain 1 Vaal Relic in Map", 2)]
     [InlineData("Expeditions contain 2(1-2) buried Strongboxes in Map", "Expeditions contain 1 buried Strongbox in Map", 2)]
     [InlineData("Expeditions contain 2(1-2) Additional Verisium Sentries in Map", "Expeditions contain 1 Additional Verisium Sentry in Map", 2)]
     [InlineData("The first 2(1-2) unearthed Runic Monsters will be Rare Monsters in Map", "The first unearthed Runic Monster will be a Rare Monster in Map", 2)]
     public void ParsePluralMapModifiers(string line, string expectedStat, double expectedValue)
     {
         var actual = parser.ParseItem(
             $@"Item Class: Tablet
Rarity: Rare
Transcendental Judgment
Ritual Tablet
--------
Item Level: 79
--------
{{ Implicit Modifier }}
Adds Ritual Altars to a Map
10 uses remaining
--------
{{ Prefix Modifier ""Treasurer's"" (Tier: 1) }}
{line}
--------
Can be used in a personal Map Device to add modifiers to a Map.");

         fixture.AssertHasStat(actual, StatCategory.Explicit, expectedStat, expectedValue);
     }

     [Fact]
     public void ParseOverseerTabletEssencesUsesTabletTradeId()
     {
         var actual = parser.ParseItem(
             @"Item Class: Tablet
Rarity: Rare
Transcendental Myth
Overseer Tablet
--------
Item Level: 82
--------
{ Implicit Modifier }
Empowers the Map Boss of a Map
10 uses remaining
--------
{ Prefix Modifier ""Abounding"" (Tier: 1) }
Map has 16(15-20)% increased Monster Rarity
{ Prefix Modifier ""Summoner's"" (Tier: 1) }
Map contains an additional Summoning Circle
{ Suffix Modifier ""of Crystals"" (Tier: 1) }
Map contains 2(1-2) additional Essences — Unscalable Value
{ Suffix Modifier ""of Shrines"" (Tier: 1) }
Map has 77(70-100)% increased chance to contain Shrines
--------
Can be used in a personal Map Device to add modifiers to a Map.");

         var stat = fixture.AssertHasStat(actual, StatCategory.Explicit, "Map contains an additional Essence", 2);
         Assert.Contains("explicit.stat_2162684861", stat!.Definitions.SelectMany(x => x.TradeIds!));
     }

     [Fact]
     public void ParseFreedomOfFaith()
     {
         var actual = parser.ParseItem(
             @"Item Class: Tablet
Rarity: Unique
Freedom of Faith
Ritual Tablet
--------
Item Level: 79
--------
{ Implicit Modifier }
Adds Ritual Altars to a Map
5 uses remaining
--------
{ Unique Modifier }
Can Reroll Favours at Ritual Altars in your Maps twice as many times — Unscalable Value
{ Unique Modifier }
Favours at Ritual Altars in Area costs 15(10-15)% increased Tribute
--------
When darkness comes, we pray for light.
When darkness is all that there is,
we pray for death.
--------
Can be used in a personal Map Device to add modifiers to a Map.");

         Assert.Equal(ItemClass.Tablet, actual.ItemClass.Type);
         Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
         Assert.Equal("Ritual Tablet", actual.TradeItem?.Type);
         Assert.Equal("Freedom of Faith", actual.TradeItem?.Name);

         Assert.Equal(79, actual.Properties.ItemLevel);

         fixture.AssertHasStat(actual, StatCategory.Pseudo, "# uses remaining (Tablets)", 5);
         fixture.AssertHasStat(actual, StatCategory.Explicit, "Can Reroll Favours at Ritual Altars in your Maps twice as many times");
         fixture.AssertHasStat(actual, StatCategory.Explicit, "Favours at Ritual Altars in Area costs #% increased Tribute", 15);
     }

     [Fact]
     public void ParseIrradiatedTablet()
     {
         var actual = parser.ParseItem(
             @"Item Class: Tablet
Rarity: Rare
Eerie Secrets
Irradiated Tablet
--------
Item Level: 79
--------
{ Implicit Modifier }
Adds Irradiated to a Map
10 uses remaining
--------
{ Prefix Modifier ""Collector's"" (Tier: 1) }
9(8-12)% increased Rarity of Items found in Map
{ Prefix Modifier ""Breeding"" (Tier: 1) }
7(5-7)% increased Pack Size in Map
{ Suffix Modifier ""of Spirits"" (Tier: 1) }
Map has 73(70-100)% increased chance to contain Azmeri Spirits
{ Suffix Modifier ""of the Exile"" (Tier: 1) }
Map has 88(70-100)% increased chance to contain Rogue Exiles
--------
Can be used in a personal Map Device to add modifiers to a Map.");

         Assert.Equal(ItemClass.Tablet, actual.ItemClass.Type);
         Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
         Assert.Equal("Irradiated Tablet", actual.TradeItem?.Type);

         Assert.Equal(79, actual.Properties.ItemLevel);

         fixture.AssertHasStat(actual, StatCategory.Pseudo, "# uses remaining (Tablets)", 10);
         fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Rarity of Items found in Map", 9);
         fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Pack Size in Map", 7);
         fixture.AssertHasStat(actual, StatCategory.Explicit, "Map has #% increased chance to contain Azmeri Spirits", 73);
         fixture.AssertHasStat(actual, StatCategory.Explicit, "Map has #% increased chance to contain Rogue Exiles", 88);
     }

     [Fact]
     public void ParseBreachTablet()
     {
         var actual = parser.ParseItem(
             @"Item Class: Tablet
Rarity: Rare
Mythic Anthem
Breach Tablet
--------
Item Level: 80
--------
{ Implicit Modifier }
Adds an Otherworldy Breach to a Map
10 uses remaining
--------
{ Prefix Modifier ""Challenger's"" (Tier: 1) }
Monsters have 15(10-15)% increased Effectiveness
{ Prefix Modifier ""Treasurer's"" (Tier: 1) }
Map contains 3(2-3) additional Rare Chests
{ Suffix Modifier ""of the Hand"" (Tier: 1) }
17(5-20)% increased Effectiveness of Rare Breach Monsters in Map
{ Suffix Modifier ""of the Invasion"" (Tier: 1) }
Unstable Breaches in Map spawn 3(1-3) additional Rare Monsters when Stabilised
--------
Can be used in a personal Map Device to add modifiers to a Map.");

         Assert.Equal(ItemClass.Tablet, actual.ItemClass.Type);
         Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
         Assert.Equal("Breach Tablet", actual.TradeItem?.Type);

         Assert.Equal(80, actual.Properties.ItemLevel);

         fixture.AssertHasStat(actual, StatCategory.Pseudo, "# uses remaining (Tablets)", 10);
         fixture.AssertHasStat(actual, StatCategory.Explicit, "Map contains an additional Rare Chest", 3);
         fixture.AssertHasStat(actual, StatCategory.Explicit, "Unstable Breaches in Map spawn an additional Rare Monster when Stabilised", 3);
     }

}
