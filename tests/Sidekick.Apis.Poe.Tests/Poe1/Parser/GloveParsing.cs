using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class GloveParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseRareGloves()
    {
        var actual = parser.ParseItem(@"Item Class: Gloves
Rarity: Rare
Death Nails
Assassin's Mitts
--------
Evasion Rating: 104
Energy Shield: 20
--------
Requirements:
Level: 58
Dex: 45
Int: 45
--------
Sockets: G
--------
Item Level: 61
--------
+18 to Intelligence
+73 to maximum Life
+14% to Lightning Resistance
0.23% of Physical Attack Damage Leeched as Mana
");

        Assert.Equal(ItemClass.Gloves, actual.Properties.ItemClass);
        Assert.Equal(Category.Armour, actual.ApiInformation.Category);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Assassin's Mitts", actual.ApiInformation.Type);
        Assert.Equal("Death Nails", actual.Name);
        Assert.NotNull(actual.Properties.Sockets);
        Assert.Single(actual.Properties.Sockets);

        actual.AssertHasModifier(ModifierCategory.Explicit, "+# to Intelligence", 18);
        actual.AssertHasModifier(ModifierCategory.Explicit, "+# to maximum Life", 73);
        actual.AssertHasModifier(ModifierCategory.Explicit, "+#% to Lightning Resistance", 14);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% of Physical Attack Damage Leeched as Mana", 0.23);
    }
}
