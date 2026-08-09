using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Game.ItemClasses;
using Sidekick.Game.Items;
using Sidekick.Game.Stats;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class BreachParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

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
    public void Wombgift()
    {
        var actual = parser.ParseItem(@"Item Class: Wombgifts
Rarity: Currency
Lavish Wombgift
--------
Item Level: 34
Requires 49 Graftblood
--------
Can grow into a Currency item on the Genesis Tree
--------
Place this item into an allocated currency item womb on the Genesis Tree. Right click to retrieve from the Genesis Tree.
");

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Lavish Wombgift", actual.TradeItem?.Type);
        Assert.Equal(34, actual.Properties.ItemLevel);
    }

    [Fact]
    public void Foulborn()
    {
        var actual = parser.ParseItem(@"Item Class: Body Armours
Rarity: Unique
Foulborn Skin of the Loyal
Simple Robe
--------
Sockets: R-B-R-R-R-B 
--------
Item Level: 74
--------
Sockets cannot be modified
100% increased Global Defences
+20% to Quality of Socketed Gems (mutated)
--------
We happily give our limbs.
A net woven to keep safe the bones of the Lords.
");

        Assert.Equal(ItemClass.BodyArmour, actual.ItemClass.Type);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Simple Robe", actual.TradeItem?.Type);
        Assert.Equal("Skin of the Loyal", actual.TradeItem?.Name);
        Assert.Equal("Foulborn Skin of the Loyal", actual.Name);
        Assert.True(actual.Properties.Foulborn);

        fixture.AssertHasStat(actual, StatCategory.Mutated, "+#% to Quality of Socketed Gems", 20);
    }
}
