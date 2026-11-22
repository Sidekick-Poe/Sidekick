using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe1.English;

[Collection(Collections.Poe1EnglishFixture)]
public class CurrencyParsing(Poe1EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

    [Fact]
    public void ChaosOrb()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Chaos Orb
--------
Stack Size: 1/10
--------
Reforges a rare item with new random modifiers
--------
Right click this item then left click a rare item to apply it.
--------
Note: ~b/o 2 blessed
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Chaos Orb", actual.TradeItem?.Type);
        actual.AssertHasNinjaExchange("chaos");

        Assert.Empty(actual.Stats);
    }

    [Fact]
    public void ExaltedOrb()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Exalted Orb
--------
Stack Size: 1/20
--------
Augments a rare item with a new random modifier
--------
Right click this item then left click a rare item to apply it. Rare items can have up to six random modifiers.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Exalted Orb", actual.TradeItem?.Type);
        actual.AssertHasNinjaExchange("exalted");

        Assert.Empty(actual.Stats);
    }

    [Fact]
    public void ClearOil()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Clear Oil
--------
Stack Size: 5/10
--------
Can be combined with other Oils at Cassia to Enchant Rings or Amulets, or to modify Blighted Maps.
Shift click to unstack.
--------
Note: ~price 1 blessed
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Clear Oil", actual.TradeItem?.Type);
    }

    [Fact]
    public void CrystallisedRancour()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Crystallised Rancour
--------
Stack Size: 2/10
--------
Can be used at the Horticrafting bench in your hideout.
Shift click to unstack.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Crystallised Rancour", actual.TradeItem?.Type);
    }

    [Fact]
    public void TheChangelingsDucat()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
The Changeling's Ducat
--------
Stack Size: 1/10
--------
Transforms into a random Ducat
--------
To appease Tsoatha, they tossed it to sea, but the storm grew ever madder.
None returned to shore. Yet the vessel sails on, and its captain is merry.
--------
Can be used as part of Allflame Crafting aboard The Sovereign.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("The Changeling's Ducat", actual.TradeItem?.Type);
    }

    [Fact]
    public void SplinterOfTul()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Splinter of Tul
--------
Stack Size: 9/100
--------
Combine 100 Splinters to create Tul's Breachstone.
Shift click to unstack.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Splinter of Tul", actual.TradeItem?.Type);
    }

    [Fact]
    public void OpulentFossil()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Opulent Fossil
--------
Stack Size: 1/20
--------
More Drop modifiers
No Tagless modifiers
--------
Place in a Resonator to influence item crafting.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Opulent Fossil", actual.TradeItem?.Type);
    }

    [Fact]
    public void WeepingEssenceofAnger()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Weeping Essence of Anger
--------
Stack Size: 5/9
--------
Upgrades a normal item to rare with one guaranteed property
Properties restricted to level 60 and below

Two Handed Melee Weapon: Adds (31-41) to (61-72) Fire Damage
Other Weapon: Adds (17-22) to (33-39) Fire Damage
Armour: (18-23)% to Fire Resistance
Quiver: (18-23)% to Fire Resistance
Belt: (18-23)% to Fire Resistance
Other Jewellery: (15-18)% increased Fire Damage
--------
Right click this item then left click a normal item to apply it.
Shift click to unstack.
--------
Note: ~price 1 fusing
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Weeping Essence of Anger", actual.TradeItem?.Type);
    }

    [Fact]
    public void RitualSplinter()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Ritual Splinter
--------
Stack Size: 17/100
--------
Combine 100 Splinters to create a Ritual Vessel.
Shift click to unstack.
--------
Note: ~price 1 alch
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Ritual Splinter", actual.TradeItem?.Type);
    }

    [Fact]
    public void RitualVessel()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Ritual Vessel
--------
Stack Size: 1/10
--------
Stores the monsters slain for the first time from a completed Ritual Altar for future use
--------
Right-click this item then left-click a Ritual Altar to store the monsters from the completed Ritual in this item. Cannot be used on a Ritual in a map opened with a Blood-Filled Vessel.
--------
Note: ~price 8 chaos
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Ritual Vessel", actual.TradeItem?.Type);
    }

    [Fact]
    public void NoxiousCatalyst()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Noxious Catalyst
--------
Stack Size: 1/10
--------
Adds quality that enhances Physical and Chaos Damage modifiers on a ring, amulet or belt
Replaces other quality types
--------
Right click this item then left click a ring, amulet or belt to apply it. Has greater effect on lower-rarity jewellery. The maximum quality is 20%.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Noxious Catalyst", actual.TradeItem?.Type);
    }

    [Fact]
    public void Shadowchew()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Rare
Shadowchew
Farric Flame Hellion Alpha
--------
Genus: Hellions
Group: Felines
Family: The Wilds
--------
Item Level: 82
--------
Burning Ground on Death
Hasted
Fertile Presence
Aspect of the Hellion
--------
Right-click to add this to your bestiary.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.TradeItem?.Name);
        Assert.Equal("Farric Flame Hellion Alpha", actual.TradeItem?.Type);
    }

    [Fact]
    public void SaqawalFirstoftheSky()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Unique
Saqawal, First of the Sky
--------
Genus: Rhexes
Group: Avians
Family: The Sands
--------
Item Level: 70
--------
Cannot be fully Slowed
--------
Right-click to add this to your bestiary.");

        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Saqawal, First of the Sky", actual.TradeItem?.Type);
        Assert.Null(actual.TradeItem?.Name);
    }

    [Fact]
    public void Duskkiller()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Rare
Duskkiller
Farric Chieftain
--------
Genus: Ape Chieftains
Group: Primates
Family: The Wilds
--------
Item Level: 81
--------
Fertile Presence
Farric Presence
Allies Deal Substantial Extra Physical Damage
Armoured
Leeches Life
Summons Apes from Trees
--------
Right-click to add this to your bestiary.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Farric Chieftain", actual.TradeItem?.Type);
    }

    [Fact]
    public void Hairyjoint()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Rare
Hairyjoint
Vivid Vulture
--------
Genus: Vultures
Group: Avians
Family: The Sands
--------
Item Level: 83
--------
Saqawine Presence
Crimson Flock
Raven Caller
Heals Allies and Suppresses Foe Recovery
Temporarily Revives
Hasted
Cannot be Stunned
10% chance not to be consumed when sacrificed at the Blood Altar
--------
Right-click to add this to your bestiary.
--------
Note: ~b/o 450 chaos
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Vivid Vulture", actual.TradeItem?.Type);
        actual.AssertHasNinjaStash("vivid-vulture");
    }

    [Fact]
    public void SimulacrumSplinter()
    {
        var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Simulacrum Splinter
--------
Stack Size: 40/300
--------
Combine 300 Splinters to create a Simulacrum.
Shift click to unstack.
--------
Note: ~price .5 chaos
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Simulacrum Splinter", actual.TradeItem?.Type);
    }
}
