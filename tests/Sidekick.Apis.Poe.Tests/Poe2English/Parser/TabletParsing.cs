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

//    [Fact]
//    public void ParseRitualTablet()
//    {
//        var actual = parser.ParseItem(
//            @"Item Class: Tablet
//Rarity: Rare
//Planar Injunction
//Ritual Tablet
//--------
//Item Level: 77
//--------
//Adds Ritual Altars to a Map (implicit)
//10 uses remaining (implicit)
//--------
//18% increased Rarity of Items found in Map
//Map has 40% increased number of Rare Monsters
//Monsters Sacrificed at Ritual Altars in Map grant 18% increased Tribute
//Deferring Favours at Ritual Altars in Map costs 25% reduced Tribute
//--------
//Can be used in a personal Map Device to add modifiers to a Map.
//");
//
//        Assert.Equal(ItemClass.Tablet, actual.ItemClass.Type);
//        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
//        Assert.Equal("Ritual Tablet", actual.Definition.TradeItem?.Type);
//
//        Assert.Equal(77, actual.Properties.ItemLevel);
//
//        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Rarity of Items found in your Maps",18);
//        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased number of Rare Monsters", 40);
//        fixture.AssertHasStat(actual, StatCategory.Explicit, "Monsters Sacrificed at Ritual Altars in Area grant #% increased Tribute", 18);
//        fixture.AssertHasStat(actual, StatCategory.Explicit, "Deferring Favours at Ritual Altars in Area costs #% increased Tribute", -25);
//    }

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

         fixture.AssertHasStat(actual, StatCategory.Explicit, "Can Reroll Favours at Ritual Altars in your Maps twice as many times");
         fixture.AssertHasStat(actual, StatCategory.Explicit, "Favours at Ritual Altars in Area costs #% increased Tribute", 15);
     }


}
