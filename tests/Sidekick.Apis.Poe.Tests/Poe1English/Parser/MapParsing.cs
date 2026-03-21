using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.Stats;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class MapParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void Tier2()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Magic
Armoured Map (Tier 2)
--------
Item Quantity: +13% (augmented)
Item Rarity: +8% (augmented)
Monster Pack Size: +5% (augmented)
--------
Item Level: 71
--------
Monster Level: 69
--------
+20% Monster Physical Damage Reduction
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.

");

        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Null(actual.Definition.Name);
        Assert.Equal("Map", actual.Definition.Type);
        Assert.Equal(2, actual.Properties.MapTier);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "+#% Monster Physical Damage Reduction", 20);
    }

    [Fact]
    public void OlmecSanctum()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Unique
Olmec's Sanctum
Map (Tier 6)
--------
Item Quantity: +179% (augmented)
--------
Item Level: 73
--------
Monster Level: 73
--------
42% more Monster Life
37% increased Monster Damage
Final Boss drops higher Level Items
--------
They flew, and leapt, and clambered over,
They crawled, and swam, and slithered under.
Still its ancient secrets await unclaimed
And of this hidden temple, only legends remain.
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Olmec's Sanctum", actual.Definition.Name);
        Assert.Equal("Map", actual.Definition.Type);
        Assert.Equal(6, actual.Properties.MapTier);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% more Monster Life", 42);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Monster Damage", 37);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Final Boss drops higher Level Items");
    }

    [Fact]
    public void MoreProperties()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Malevolent Secrets
Map (Tier 16)
--------
Item Quantity: +101% (augmented)
Item Rarity: +62% (augmented)
Monster Pack Size: +39% (augmented)
More Maps: +110% (augmented)
More Scarabs: +35% (augmented)
More Currency: +45% (augmented)
--------
Item Level: 85
--------
Monster Level: 83
--------
Area is Influenced by the Originator's Memories (implicit)
--------
+40% Monster Physical Damage Reduction
Monsters have +1 to Maximum Endurance Charges
Area has patches of Consecrated Ground
Monsters gain an Endurance Charge when hit
Monsters inflict 2 Grasping Vines on Hit
Monsters gain 73% of Maximum Life as Extra Maximum Energy Shield
Monsters have +60% chance to Suppress Spell Damage
Players are targeted by a Meteor when they use a Flask
Players have 27% less Defences
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
--------
Corrupted
");

        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.Definition.Name);
        Assert.Equal("Map", actual.Definition.Type);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(101, actual.Properties.ItemQuantity);
        Assert.Equal(62, actual.Properties.ItemRarity);
        Assert.Equal(39, actual.Properties.MonsterPackSize);
        Assert.Equal(110, actual.Properties.MoreMaps);
        Assert.Equal(35, actual.Properties.MoreScarabs);
        Assert.Equal(45, actual.Properties.MoreCurrency);
        Assert.True(actual.Properties.Corrupted);
    }

    [Fact]
    public void QualityCurrency()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Map (Tier 16)
--------
More Currency: +50% (augmented)
Quality (Currency): +20% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(50, actual.Properties.MoreCurrency);
        Assert.Equal(20, actual.Properties.QualityCurrency);
    }

    [Fact]
    public void QualityScarabs()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Map (Tier 16)
--------
More Scarabs: +50% (augmented)
Quality (Scarabs): +20% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(50, actual.Properties.MoreScarabs);
        Assert.Equal(20, actual.Properties.QualityScarabs);
    }

    [Fact]
    public void QualityCards()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Map (Tier 16)
--------
More Divination Cards: +50% (augmented)
Quality (Divination Cards): +20% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(50, actual.Properties.MoreCards);
        Assert.Equal(20, actual.Properties.QualityCards);
    }

    [Fact]
    public void QualityPackSize()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Map (Tier 16)
--------
Monster Pack Size: +10% (augmented)
Quality (Pack Size): +20% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(10, actual.Properties.MonsterPackSize);
        Assert.Equal(20, actual.Properties.QualityPackSize);
    }

    [Fact]
    public void QualityRarity()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Map (Tier 16)
--------
Item Rarity: +40% (augmented)
Quality (Rarity): +20% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.Properties.ItemClass);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(40, actual.Properties.ItemRarity);
        Assert.Equal(20, actual.Properties.QualityRarity);
    }
}
