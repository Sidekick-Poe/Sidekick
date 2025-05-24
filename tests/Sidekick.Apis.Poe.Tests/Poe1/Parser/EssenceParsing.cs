using Sidekick.Apis.Poe.Trade;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class EssenceParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseWeepingEssenceOfAnger()
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

        Assert.Equal("currency", actual.Header.ApiItemCategory);
        Assert.Equal(Rarity.Currency, actual.Header.Rarity);
        Assert.Equal(Category.Currency, actual.Header.Category);
        Assert.Equal("Weeping Essence of Anger", actual.Header.ApiType);
    }
}
