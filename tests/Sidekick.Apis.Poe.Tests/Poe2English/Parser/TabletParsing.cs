using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.ItemClasses;
using Sidekick.Data.Items;
using Sidekick.Data.Stats;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe2English.Parser;

[Collection(Collections.Poe2EnglishFixture)]
public class TabletParsing(Poe2EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

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
        Assert.Equal("Ritual Tablet", actual.Definition.TradeItem?.Type);

        Assert.Equal(79, actual.Properties.ItemLevel);

        fixture.AssertHasFuzzyStat(actual, StatCategory.Implicit, "Adds Ritual Altars to a Map \n# use remaining", 10);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Rarity of Items found in Map",11);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Experience gain in Map", 17);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Map contains an additional Shrine");
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Deferring Favours at Ritual Altars in Area costs #% increased Tribute", -28);
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
         Assert.Equal("Ritual Tablet", actual.Definition.TradeItem?.Type);
         Assert.Equal("Freedom of Faith", actual.Definition.TradeItem?.Name);

         Assert.Equal(79, actual.Properties.ItemLevel);

         fixture.AssertHasFuzzyStat(actual, StatCategory.Implicit, "Adds Ritual Altars to a Map \n# use remaining", 5);
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
         Assert.Equal("Irradiated Tablet", actual.Definition.TradeItem?.Type);

         Assert.Equal(79, actual.Properties.ItemLevel);

         fixture.AssertHasFuzzyStat(actual, StatCategory.Implicit, "Adds Irradiated to a Map \n# use remaining", 10);
         fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Rarity of Items found in Map", 9);
         fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Pack Size in Map", 7);
         fixture.AssertHasStat(actual, StatCategory.Explicit, "Map has #% increased chance to contain Azmeri Spirits", 73);
         fixture.AssertHasStat(actual, StatCategory.Explicit, "Map has #% increased chance to contain Rogue Exiles", 88);
     }

}
