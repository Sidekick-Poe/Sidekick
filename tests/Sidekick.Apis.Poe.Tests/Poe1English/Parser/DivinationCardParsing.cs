using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class DivinationCardParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseSaintTreasure()
    {
        var actual = parser.ParseItem(@"Item Class: Divination Cards
Rarity: Divination Card
The Saint's Treasure
--------
Stack Size: 1/10
--------
2x Exalted Orb
--------
Publicly, he lived a pious and chaste life of poverty. Privately, tithes and tributes made him and his lascivious company very comfortable indeed.
");

        Assert.Equal(ItemClass.DivinationCard, actual.Properties.ItemClass);
        Assert.Equal(Rarity.DivinationCard, actual.Properties.Rarity);
        Assert.Null(actual.ApiInformation.Name);
        Assert.Equal("The Saint's Treasure", actual.ApiInformation.Type);
    }

    [Fact]
    public void ParseLordOfCelebration()
    {
        var actual = parser.ParseItem(@"Item Class: Divination Cards
Rarity: Divination Card
The Lord of Celebration
--------
Stack Size: 1/4
--------
Sceptre of Celebration
Shaper Item
--------
Though they were a pack of elite combatants, the Emperor's royal guards were not ready to face one of his notorious parties.");

        Assert.Equal(ItemClass.DivinationCard, actual.Properties.ItemClass);
        Assert.Equal(Rarity.DivinationCard, actual.Properties.Rarity);
        Assert.Null(actual.ApiInformation.Name);
        Assert.Equal("The Lord of Celebration", actual.ApiInformation.Type);
        Assert.False(actual.Properties.Influences.Crusader);
        Assert.False(actual.Properties.Influences.Elder);
        Assert.False(actual.Properties.Influences.Hunter);
        Assert.False(actual.Properties.Influences.Redeemer);
        Assert.False(actual.Properties.Influences.Shaper);
        Assert.False(actual.Properties.Influences.Warlord);
    }

    [Fact]
    public void BoonOfJustice()
    {
        var actual = parser.ParseItem(@"Item Class: Divination Cards
Rarity: Divination Card
Boon of Justice
--------
Stack Size: 1/6
--------
Offering to the Goddess
--------
Some gifts are obligations while others are simply opportunities.
--------
Note: ~price 1 blessed
");

        Assert.Equal(ItemClass.DivinationCard, actual.Properties.ItemClass);
        Assert.Equal(Rarity.DivinationCard, actual.Properties.Rarity);
        Assert.Equal("Boon of Justice", actual.ApiInformation.Type);
    }
}
