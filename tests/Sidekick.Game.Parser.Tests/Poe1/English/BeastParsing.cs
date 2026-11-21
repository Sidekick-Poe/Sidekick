using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
using Xunit;
namespace Sidekick.Game.Parser.Tests.Poe1.English;

[Collection(Collections.Poe1EnglishFixture)]
public class BeastParsing(Poe1EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

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
}
