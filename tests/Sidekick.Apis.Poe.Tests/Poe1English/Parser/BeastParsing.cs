using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class BeastParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseRareBeast()
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

        Assert.Equal(ItemClass.Currency, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Null(actual.ApiInformation.Name);
        Assert.Equal("Farric Flame Hellion Alpha", actual.ApiInformation.Type);
    }

    [Fact]
    public void ParseUniqueBeast()
    {
        var parsedRareBeast = parser.ParseItem(@"Item Class: Stackable Currency
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

        Assert.Equal(Rarity.Unique, parsedRareBeast.Properties.Rarity);
        Assert.Equal("Saqawal, First of the Sky", parsedRareBeast.ApiInformation.Type);
        Assert.Null(parsedRareBeast.ApiInformation.Name);
    }

    [Fact]
    public void FarricChieftain()
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

        Assert.Equal(ItemClass.Currency, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Farric Chieftain", actual.ApiInformation.Type);
    }
}
