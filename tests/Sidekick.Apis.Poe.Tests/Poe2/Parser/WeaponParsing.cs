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

    [Fact]
    public void ParseColdBow()
    {
        var actual = parser.ParseItem(
            @"Item Class: Bows
Rarity: Rare
Brood Fletch
Expert Composite Bow
--------
Quality: +14% (augmented)
Physical Damage: 177-289 (augmented)
Cold Damage: 39-75 (augmented)
Critical Hit Chance: 5.00%
Attacks per Second: 1.20
--------
Requirements:
Level: 72
Dex: 125 (augmented)
--------
Sockets: S S 
--------
Item Level: 76
--------
40% increased Physical Damage (rune)
--------
73% increased Physical Damage
Adds 24 to 37 Physical Damage
Adds 39 to 75 Cold Damage
35% reduced Attribute Requirements
+3 to Level of all Projectile Skills
Leeches 4.02% of Physical Damage as Mana
");

        Assert.Equal(Category.Weapon, actual.Metadata.Category);
        Assert.Equal("Ashen Staff", actual.Metadata.Type);
        Assert.Null(actual.Metadata.Name);
        Assert.Equal(60, actual.Properties.ItemLevel);

        actual.AssertHasModifier(ModifierCategory.Explicit, "# to maximum Mana", 148);
        actual.AssertHasModifier(ModifierCategory.Explicit, "# to Intelligence", 20);
    }

    [Fact]
    public void ParseElementalCrossbow()
    {
        var actual = parser.ParseItem(
            @"Item Class: Crossbows
Rarity: Rare
Blood Core
Advanced Forlorn Crossbow
--------
Physical Damage: 23-92
Elemental Damage: 20-31 (augmented), 2-91 (augmented)
Critical Hit Chance: 5.00%
Attacks per Second: 1.60
Reload Time: 0.80
--------
Requirements:
Level: 62
Str: 78 (unmet)
Dex: 78 (unmet)
--------
Item Level: 69
--------
Adds 20 to 31 Fire Damage
Adds 2 to 91 Lightning Damage
Grants 3 Life per Enemy Hit");

        Assert.Equal(Category.Weapon, actual.Metadata.Category);
        Assert.Equal("Ashen Staff", actual.Metadata.Type);
        Assert.Null(actual.Metadata.Name);
        Assert.Equal(60, actual.Properties.ItemLevel);

        actual.AssertHasModifier(ModifierCategory.Explicit, "# to maximum Mana", 148);
        actual.AssertHasModifier(ModifierCategory.Explicit, "# to Intelligence", 20);
    }

}
