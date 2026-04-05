using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.ItemClasses;
using Sidekick.Data.Items;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

/**
 * Poe.ninja does not include Item Class as the first line. This class tests special cases to ensure accurate parsing.
 */
[Collection(Collections.Poe1EnglishFixture)]
public class PoeNinjaParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void SacredChainmail()
    {
        var actual = parser.ParseItem(@"Rarity: Rare
Tempest Sanctuary
Sacred Chainmail
--------
Item Level: 86
--------
Quality: +20%
Armour: 985
Energy Shield: 198
--------
Requirements:
Level: 84
Str: 173
Int: 173
--------
Sockets: B-B-B-B-B-R
--------
--------
5% Chance to Block Attack Damage (implicit)
17% increased Lightning Damage (implicit)
--------
+73 to maximum Mana (fractured)
+148 to maximum Life
+47% to Lightning Resistance
+32% to Chaos Resistance
7% additional Physical Damage Reduction
7% increased maximum Life (crafted)
8% increased maximum Mana (crafted)
--------
Fractured Item
");

        Assert.Equal(ItemClass.BodyArmour, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Sacred Chainmail", actual.Definition.TradeItem?.Type);

        Assert.Equal(985, actual.Properties.ArmourWithQuality);
    }

}
