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
        Assert.Equal("Expert Composite Bow", actual.Metadata.Type);
        Assert.Null(actual.Metadata.Name);
        Assert.Equal(76, actual.Properties.ItemLevel);

        // Verify physical damage
        Assert.Equal(177, actual.Properties.PhysicalDamage?.Min);
        Assert.Equal(289, actual.Properties.PhysicalDamage?.Max);
        Assert.Equal(279.6, actual.Properties.PhysicalDps);

        // Verify elemental damages
        Assert.Equal(39, actual.Properties.ColdDamage?.Min);
        Assert.Equal(75, actual.Properties.ColdDamage?.Max);
        Assert.Equal(68.4, actual.Properties.ElementalDps);

        actual.AssertHasModifier(ModifierCategory.Rune, "#% increased Physical Damage", 40);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Physical Damage", 73);
        actual.AssertHasModifier(ModifierCategory.Explicit, "Adds # to # Physical Damage", 24, 37);
        actual.AssertHasModifier(ModifierCategory.Explicit, "Adds # to # Cold Damage", 39, 75);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Attribute Requirements", 35);
        actual.AssertHasModifier(ModifierCategory.Explicit, "# to Level of all Projectile Skills", 3);
        actual.AssertHasModifier(ModifierCategory.Explicit, "Leeches #% of Physical Damage as Mana", 4.02);;
    }

    [Fact]
    public void ParseElementalCrossbow()
    {
        var actual = parser.ParseItem(@"Item Class: Crossbows
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
Str: 78
Dex: 78
--------
Item Level: 69
--------
Adds 20 to 31 Fire Damage
Adds 2 to 91 Lightning Damage
Grants 3 Life per Enemy Hit
");

        Assert.Equal(Category.Weapon, actual.Metadata.Category);
        Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
        Assert.Equal("Advanced Forlorn Crossbow", actual.Metadata.Type);
        Assert.Equal("Blood Core", actual.Header.Name);

        // Verify physical damage
        Assert.Equal(23, actual.Properties.PhysicalDamage?.Min);
        Assert.Equal(92, actual.Properties.PhysicalDamage?.Max);
        Assert.Equal(92.0, actual.Properties.PhysicalDps);

        // Verify elemental damages
        Assert.Equal(20, actual.Properties.FireDamage?.Min);
        Assert.Equal(31, actual.Properties.FireDamage?.Max);

        Assert.Equal(2, actual.Properties.LightningDamage?.Min);
        Assert.Equal(91, actual.Properties.LightningDamage?.Max);

        // Verify DPS calculations
        Assert.Equal(115.2, actual.Properties.ElementalDps); // ((20+31)/2 + (2+91)/2) * 1.60
        AssertHelper.CloseEnough(207.2, actual.Properties.TotalDps); // 92.0 + 115.2
    }

    [Fact]
    public void ParseChaosDamageWeapon()
    {
        var actual = parser.ParseItem(@"Item Class: Unknown
Rarity: Magic
Advanced Cultist Bow of the Parched
--------
Bow
Chaos Damage: 41-69
Critical Strike Chance: 5.00%
Attacks per Second: 1.20
Weapon Range: 11
--------
Requirements:
Level: 59
Dex: 135
--------
Item Level: 60
--------
Leeches 5.82% of Physical Damage as Mana");

        Assert.Equal(Category.Weapon, actual.Metadata.Category);
        Assert.Equal(Rarity.Magic, actual.Metadata.Rarity);
        Assert.Equal("Advanced Cultist Bow", actual.Metadata.Type);

        // Verify the chaos damage range is parsed correctly
        Assert.Equal(41, actual.Properties.ChaosDamage?.Min);
        Assert.Equal(69, actual.Properties.ChaosDamage?.Max);

        // Verify DPS calculations
        Assert.Equal(66.0, actual.Properties.ChaosDps);
        Assert.Equal(66.0, actual.Properties.TotalDps);
    }
}
