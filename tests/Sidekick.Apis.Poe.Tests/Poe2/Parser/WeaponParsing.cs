using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe2.Parser;

[Collection(Collections.Poe2Parser)]
public class WeaponParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseStaff()
    {
        var actual = parser.ParseItem(
            @"Item Class: Staves
Rarity: Magic
Chalybeous Ashen Staff of the Augur
--------
Requirements:
Level: 58
Int: 133 (unmet)
--------
Item Level: 60
--------
+148 to maximum Mana
+20 to Intelligence
");

        Assert.Equal(Category.Weapon, actual.Metadata.Category);
        Assert.Equal("Ashen Staff", actual.Metadata.Type);
        Assert.Null(actual.Metadata.Name);
        Assert.Equal(60, actual.Properties.ItemLevel);

        actual.AssertHasModifier(ModifierCategory.Explicit, "# to maximum Mana", 148);
        actual.AssertHasModifier(ModifierCategory.Explicit, "# to Intelligence", 20);
    }
}
