using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class DeliriumParsing(Poe1EnglishFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

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

        Assert.Equal(ItemClass.Currency, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal(Category.Currency, actual.ApiInformation.Category);
        Assert.Equal("Simulacrum Splinter", actual.ApiInformation.Type);
    }

    [Fact]
    public void SmallClusterJewel()
    {
        var actual = parser.ParseItem(@"Item Class: Jewels
Rarity: Rare
Oblivion Ruin
Small Cluster Jewel
--------
Item Level: 45
--------
Adds 2 Passive Skills (enchant)
Added Small Passive Skills grant: 15% increased Evasion Rating (enchant)
--------
Added Small Passive Skills also grant: +3 to Maximum Life
Added Small Passive Skills also grant: +3 to Strength
1 Added Passive Skill is Readiness
--------
Place into an allocated Small, Medium or Large Jewel Socket on the Passive Skill Tree. Added passives do not interact with jewel radiuses. Right click to remove from the Socket.
--------
Note: ~b/o 1 chance
");

        Assert.Equal(ItemClass.Jewel, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal(Category.Jewel, actual.ApiInformation.Category);
        Assert.Equal("Small Cluster Jewel", actual.ApiInformation.Type);

        Assert.Equal(2, actual.Properties.ClusterJewelPassiveCount);
    }

}
