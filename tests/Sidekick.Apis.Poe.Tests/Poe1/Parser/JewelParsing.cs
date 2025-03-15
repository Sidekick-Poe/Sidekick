using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser;

[Collection(Collections.Poe1Parser)]
public class JewelParsing(ParserFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void ParseJewelBlightCut()
    {
        var actual = parser.ParseItem(@"Item Class: Unknown
Rarity: Rare
Blight Cut
Cobalt Jewel
--------
Item Level: 68
--------
+8 to Strength and Intelligence
14% increased Spell Damage while Dual Wielding
19% increased Burning Damage
15% increased Damage with Wands
--------
Place into an allocated Jewel Socket on the Passive Skill Tree.Right click to remove from the Socket.
");

        Assert.Equal(Category.Jewel, actual.Header.Category);
        Assert.Equal(Rarity.Rare, actual.Header.Rarity);
        Assert.Equal("Cobalt Jewel", actual.Header.ApiType);
        Assert.Equal("Blight Cut", actual.Header.Name);
        Assert.Equal(68, actual.Properties.ItemLevel);

        actual.AssertHasModifier(ModifierCategory.Explicit, "+# to Strength and Intelligence", 8);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Spell Damage while Dual Wielding", 14);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Burning Damage", 19);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Damage with Wands", 15);
    }

    [Fact]
    public void ChimericSliver()
    {
        var actual = parser.ParseItem(@"Item Class: Jewels
Rarity: Rare
Chimeric Sliver
Large Cluster Jewel
--------
Requirements:
Level: 54
--------
Item Level: 69
--------
Adds 11 Passive Skills (enchant)
2 Added Passive Skills are Jewel Sockets (enchant)
Added Small Passive Skills grant: Axe Attacks deal 12% increased Damage with Hits and Ailments (enchant)
Added Small Passive Skills grant: Sword Attacks deal 12% increased Damage with Hits and Ailments (enchant)
--------
Added Small Passive Skills also grant: +7 to Maximum Mana
Added Small Passive Skills also grant: Regenerate 0.1% of Life per Second
1 Added Passive Skill is Heavy Hitter
--------
Place into an allocated Large Jewel Socket on the Passive Skill Tree. Added passives do not interact with jewel radiuses. Right click to remove from the Socket.
--------
Note: ~b/o 1 chaos
");

        Assert.Equal(Category.Jewel, actual.Header.Category);
        Assert.Equal(Rarity.Rare, actual.Header.Rarity);
        Assert.Equal("Large Cluster Jewel", actual.Header.ApiType);
        Assert.Equal("Chimeric Sliver", actual.Header.Name);
        Assert.Equal(69, actual.Properties.ItemLevel);

        actual.AssertHasModifier(ModifierCategory.Enchant, "Added Small Passive Skills grant: Axe Attacks deal 12% increased Damage with Hits and Ailments\nAdded Small Passive Skills grant: Sword Attacks deal 12% increased Damage with Hits and Ailments");
        actual.AssertHasModifier(ModifierCategory.Enchant, "# Added Passive Skills are Jewel Sockets", 2);
        actual.AssertHasModifier(ModifierCategory.Enchant, "Adds # Passive Skills", 11);
    }

    [Fact]
    public void ViridianJewel()
    {
        var actual = parser.ParseItem(@"Item Class: Jewels
Rarity: Rare
Phoenix Thirst
Viridian Jewel
--------
Item Level: 85
--------
8% increased Damage
15% increased Damage with Axes
7% increased Attack Speed with Axes
14% increased Global Accuracy Rating
--------
Place into an allocated Jewel Socket on the Passive Skill Tree. Right click to remove from the Socket.
");

        Assert.Equal("jewel", actual.Header.ItemCategory);
        Assert.Equal(Rarity.Rare, actual.Header.Rarity);
        Assert.Equal(Category.Jewel, actual.Header.Category);
        Assert.Equal("Viridian Jewel", actual.Header.ApiType);
    }

    [Fact]
    public void Twwt()
    {
        var actual = parser.ParseItem(@"Item Class: Jewels
Rarity: Unique
That Which Was Taken
Crimson Jewel
--------
Limited to: 1
--------
Requirements:
Level: 48
--------
Item Level: 86
--------
8% increased Strength
16% chance to gain Onslaught for 4 seconds on Kill
Enemies you Kill that are affected by Elemental Ailments
grant 32% increased Flask Charges
Cannot take Reflected Elemental Damage
--------
Faith given under false pretenses still carries the same power.
--------
Place into an allocated Jewel Socket on the Passive Skill Tree. Right click to remove from the Socket.
");

        Assert.Equal("jewel", actual.Header.ItemCategory);
        Assert.Equal(Rarity.Unique, actual.Header.Rarity);
        Assert.Equal(Category.Jewel, actual.Header.Category);
        Assert.Equal("Crimson Jewel", actual.Header.ApiType);

        actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Strength", 8);
        actual.AssertHasModifier(ModifierCategory.Explicit, "#% chance to gain Onslaught for 4 seconds on Kill", 16, 4);
        actual.AssertHasModifier(ModifierCategory.Explicit, "Enemies you Kill that are affected by Elemental Ailments\ngrant #% increased Flask Charges", 32);
        actual.AssertHasModifier(ModifierCategory.Explicit, "Cannot take Reflected Elemental Damage");
    }
}
