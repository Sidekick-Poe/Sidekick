using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class GloveParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseRareGloves()
    {
        var actual = parser.ParseItem(@"Item Class: Unknown
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

        Assert.Equal(Category.Armour, actual.Header.Category);
        Assert.Equal(Rarity.Rare, actual.Header.Rarity);
        Assert.Equal("Assassin's Mitts", actual.Header.ApiType);
        Assert.Equal("Death Nails", actual.Header.Name);
        Assert.NotNull(actual.Properties.Sockets);
        Assert.Single(actual.Properties.Sockets);

        actual.AssertHasModifier(ModifierCategory.Explicit, "+# to Intelligence", 18);
        actual.AssertHasModifier(ModifierCategory.Explicit, "+# to maximum Life", 73);
        actual.AssertHasModifier(ModifierCategory.Explicit, "+#% to Lightning Resistance", 14);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% of Physical Attack Damage Leeched as Mana", 0.23);
    }
}
