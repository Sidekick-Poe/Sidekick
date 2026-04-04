using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.ItemDefinitions;
using Sidekick.Data.Items;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class BeltParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseBroodCircle()
    {
        var actual = parser.ParseItem(@"Item Class: Belts
Rarity: Rare
Honour Leash
Cord Belt
--------
Quality (Resistance Modifiers): +20% (augmented)
--------
Requirements:
Level: 67
--------
Item Level: 83
--------
Allocates As The Mountain (enchant)
--------
Can be Anointed (implicit)
--------
+33% to Chaos Resistance (fractured)
+25 to Strength
+136 to maximum Life
+66 to maximum Mana
+56% to Fire Resistance
17% increased Damage (crafted)
--------
Split
--------
Fractured Item
");

        Assert.Equal(ItemClass.Belt, actual.Definition.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Cord Belt", actual.Definition.TradeItem?.Type);

        Assert.Equal(83, actual.Properties.ItemLevel);
        Assert.True(actual.Properties.Split);
        Assert.True(actual.Properties.Fractured);
    }
}
