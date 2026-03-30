using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Items;
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

        Assert.Equal(ItemClass.Map, actual.ItemClass);
        Assert.Equal(Rarity.Magic, actual.Properties.Rarity);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal("Map", actual.Definition.TradeItem?.Type);
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

        Assert.Equal(ItemClass.Map, actual.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Olmec's Sanctum", actual.Definition.TradeItem?.Name);
        Assert.Equal("Map", actual.Definition.TradeItem?.Type);
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

        Assert.Equal(ItemClass.Map, actual.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal("Map", actual.Definition.TradeItem?.Type);
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

        Assert.Equal(ItemClass.Map, actual.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal("Map", actual.Definition.TradeItem?.Type);
        Assert.Equal(16, actual.Properties.MapTier);
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

        Assert.Equal(ItemClass.Map, actual.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal("Map", actual.Definition.TradeItem?.Type);
        Assert.Equal(16, actual.Properties.MapTier);
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

        Assert.Equal(ItemClass.Map, actual.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal("Map", actual.Definition.TradeItem?.Type);
        Assert.Equal(16, actual.Properties.MapTier);
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

        Assert.Equal(ItemClass.Map, actual.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal("Map", actual.Definition.TradeItem?.Type);
        Assert.Equal(16, actual.Properties.MapTier);
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

        Assert.Equal(ItemClass.Map, actual.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal("Map", actual.Definition.TradeItem?.Type);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(40, actual.Properties.ItemRarity);
        Assert.Equal(20, actual.Properties.QualityRarity);
    }

    [Fact]
    public void NightmareMap()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Terror Grind
Nightmare Map
--------
Item Quantity: +78% (augmented)
Item Rarity: +45% (augmented)
Monster Pack Size: +41% (augmented)
More Currency: +47% (augmented)
--------
Item Level: 83
--------
Monster Level: 83
--------
93% more Monster Life
Monsters reflect 18% of Physical Damage
Monsters gain a Power Charge on Hit
Monsters take 36% reduced Extra Damage from Critical Strikes
Monsters' Attacks have 60% chance to Impale on Hit
Rare monsters in area Temporarily Revive on death
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
--------
Modifiable only with Chaos Orbs, Vaal Orbs, Delirium Orbs and Chisels
");

        Assert.Equal(ItemClass.Map, actual.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal("Nightmare Map", actual.Definition.TradeItem?.Type);
        Assert.Equal(78, actual.Properties.ItemQuantity);
        Assert.Equal(45, actual.Properties.ItemRarity);
        Assert.Equal(41, actual.Properties.MonsterPackSize);
        Assert.Equal(47, actual.Properties.MoreCurrency);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% more Monster Life", 93);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Monsters reflect #% of Physical Damage", 18);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Monsters have #% chance to gain a Power Charge on Hit");
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Monsters take #% reduced Extra Damage from Critical Strikes", 36);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Monsters' Attacks have #% chance to Impale on Hit", 60);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Rare monsters in area Temporarily Revive on death");
    }

    [Fact]
    public void OriginatorMap()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Foe Portent
Map (Tier 16)
--------
Item Quantity: +113% (augmented)
Item Rarity: +117% (augmented)
Monster Pack Size: +72% (augmented)
More Maps: +35% (augmented)
More Currency: +64% (augmented)
--------
Item Level: 85
--------
Monster Level: 83
--------
Area is Influenced by the Originator's Memories (implicit)
--------
Area has patches of Chilled Ground
Monsters have 100% increased Area of Effect
34% increased Monster Damage
+50% Monster Physical Damage Reduction
Monsters gain an Endurance Charge on Hit
Debuffs on Monsters expire 100% faster
+35% Monster Chaos Resistance
+55% Monster Elemental Resistances
Rare monsters in area are Shaper-Touched
Area contains Drowning Orbs
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
--------
Corrupted
");

        Assert.Equal(ItemClass.Map, actual.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal("Map", actual.Definition.TradeItem?.Type);
        Assert.Equal(16, actual.Properties.MapTier);
        Assert.Equal(113, actual.Properties.ItemQuantity);
        Assert.Equal(117, actual.Properties.ItemRarity);
        Assert.Equal(72, actual.Properties.MonsterPackSize);
        Assert.Equal(35, actual.Properties.MoreMaps);
        Assert.Equal(64, actual.Properties.MoreCurrency);

        fixture.AssertHasStat(actual, StatCategory.Implicit, "Area is Influenced by the Originator's Memories");
    }

    [Fact]
    public void ValdoMap()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Rare
Filthy Abode
Valdo Map
--------
Map Area: Arsenal
Reward: Foil The Dancing Dervish
Item Rarity: +50% (augmented)
Monster Pack Size: +26% (augmented)
--------
Chance for dropped Maps to convert to:
Scarab: 18% (augmented)
--------
Item Level: 100
--------
Monster Level: 84
--------
Slaying Enemies in a kill streak grants Rampage bonuses
All Strongboxes in Area must be opened to claim Reward
Area contains 12 additional packs of Untainted Wild Animals
Players gain Rare Monster Modifiers for 20 seconds on Kill
Rare Monsters each have 3 additional Modifiers
Area contains 10 additional guarded Exquisite Vaal Vessels
Area contains enemies of all influence types
--------
Travel to this Map by using it in a personal Map Device. Maps can only be used once. Defeat 90% of all monsters in this Map, including all Rare and Unique enemies to obtain the Reward. The area created is not affected by your Atlas Passive Tree, and cannot be augmented via the Map Device.
--------
Unmodifiable
--------
Foil (Celestial Amethyst)
");

        Assert.Equal(ItemClass.Map, actual.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal("Valdo Map", actual.Definition.TradeItem?.Type);
        Assert.Equal("Foil The Dancing Dervish", actual.Properties.Reward);
        Assert.Equal(50, actual.Properties.ItemRarity);
        Assert.Equal(26, actual.Properties.MonsterPackSize);
    }

    [Fact]
    public void BlightedMap()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Blighted Map (Tier 13)
--------
Map Area: Necropolis
--------
Item Level: 83
--------
Monster Level: 80
--------
Area is infested with Fungal Growths (implicit)
Map's Item Quantity Modifiers also affect Blight Chest count at 25% value (implicit)
Can be Anointed up to 3 times (implicit)
Natural inhabitants of this area have been removed (implicit)
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
");

        Assert.Equal(ItemClass.Map, actual.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal("Map", actual.Definition.TradeItem?.Type);
        Assert.Equal(13, actual.Properties.MapTier);
        Assert.True(actual.Properties.Blighted);
    }

    [Fact]
    public void BlightedMapTier3()
    {
        var actual = parser.ParseItem(@"Item Class: Maps
Rarity: Normal
Blighted Map (Tier 3)
--------
Map Area: Tropical Island
--------
Item Level: 79
--------
Monster Level: 70
--------
Area is infested with Fungal Growths (implicit)
Map's Item Quantity Modifiers also affect Blight Chest count at 25% value (implicit)
Can be Anointed up to 3 times (implicit)
Natural inhabitants of this area have been removed (implicit)
--------
Travel to a Map by using this in a personal Map Device. Maps can only be used once.
--------
Note: ~b/o 1 chaos
");

        Assert.Equal(ItemClass.Map, actual.ItemClass);
        Assert.Equal(Rarity.Normal, actual.Properties.Rarity);
        Assert.Null(actual.Definition.TradeItem?.Name);
        Assert.Equal("Map", actual.Definition.TradeItem?.Type);
        Assert.Equal(3, actual.Properties.MapTier);
        Assert.True(actual.Properties.Blighted);
    }
}
