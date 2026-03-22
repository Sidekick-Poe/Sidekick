using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Data.Items;
using Sidekick.Data.Stats;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class JewelParsing(Poe1EnglishFixture fixture)
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

        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Cobalt Jewel", actual.Definition.Type);
        Assert.Equal("Blight Cut", actual.Name);
        Assert.Equal(68, actual.Properties.ItemLevel);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "+# to Strength and Intelligence", 8);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Spell Damage while Dual Wielding", 14);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Burning Damage", 19);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Damage with Wands", 15);
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

        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Large Cluster Jewel", actual.Definition.Type);
        Assert.Equal("Chimeric Sliver", actual.Name);
        Assert.Equal(69, actual.Properties.ItemLevel);

        fixture.AssertHasStat(actual, StatCategory.Enchant, "Added Small Passive Skills grant: #", "Axe Attacks deal 12% increased Damage with Hits and Ailments\nSword Attacks deal 12% increased Damage with Hits and Ailments");
        fixture.AssertHasStat(actual, StatCategory.Enchant, "# Added Passive Skills are Jewel Sockets", 2);
        fixture.AssertHasStat(actual, StatCategory.Enchant, "Adds # Passive Skills", 11);
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

        Assert.Equal(ItemClass.Jewel, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Viridian Jewel", actual.Definition.Type);
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

        Assert.Equal(ItemClass.Jewel, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Crimson Jewel", actual.Definition.Type);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased Strength", 8);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% chance to gain Onslaught for 4 seconds on Kill", 16);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Enemies you Kill that are affected by Elemental Ailments\ngrant #% increased Flask Charges", 32);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "Cannot take Reflected Elemental Damage");
    }

    [Fact]
    public void ConvertWatcherEyeMod()
    {
        var actual = parser.ParseItem(@"Item Class: Jewels
Rarity: Unique
Watcher's Eye
Prismatic Jewel
--------
Limited to: 1
--------
Item Level: 85
--------
6% increased maximum Energy Shield
6% increased maximum Life
6% increased maximum Mana
32% of Physical Damage Converted to Cold Damage while affected by Hatred
+6% chance to Evade Attack Hits while affected by Grace
--------
One by one, they stood their ground against a creature 
they had no hope of understanding, let alone defeating,
and one by one, they became a part of it.
--------
Place into an allocated Jewel Socket on the Passive Skill Tree. Right click to remove from the Socket.
");

        Assert.Equal(ItemClass.Jewel, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);
        Assert.Equal("Prismatic Jewel", actual.Definition.Type);

        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased maximum Energy Shield", 6);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased maximum Life", 6);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% increased maximum Mana", 6);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "#% of Physical Damage Converted to Cold Damage while affected by Hatred", 32);
        fixture.AssertHasStat(actual, StatCategory.Explicit, "+#% chance to Evade Attack Hits while affected by Grace", 6);
    }

    [Fact]
    public void AbyssJewel()
    {
        var actual = parser.ParseItem(@"Item Class: Abyss Jewels
Rarity: Rare
Whispering Leer
Hypnotic Eye Jewel
--------
Abyss
--------
Requirements:
Level: 52
--------
Item Level: 69
--------
Adds 12 to 18 Fire Damage to Spells
Adds 16 to 23 Cold Damage to Spells
2 to 20 Added Spell Lightning Damage while wielding a Two Handed Weapon
9 to 15 Added Spell Physical Damage while wielding a Two Handed Weapon
--------
Place into an Abyssal Socket on an Item or into an allocated Jewel Socket on the Passive Skill Tree. Right click to remove from the Socket.
--------
Note: ~price 1 alch
");

        Assert.Equal(ItemClass.AbyssJewel, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Hypnotic Eye Jewel", actual.Definition.Type);
    }

    [Fact]
    public void NoAbyssStat()
    {
        var actual = parser.ParseItem(@"Item Class: Abyss Jewels
Rarity: Rare
Hollow Gaze
Ghastly Eye Jewel
--------
Abyss
--------
Requirements:
Level: 59
--------
Item Level: 80
--------
+31 to maximum Life
+14% to Fire Resistance
Minions have 15% chance to Poison Enemies on Hit
Minions deal 2 to 53 additional Lightning Damage
--------
Place into an Abyssal Socket on an Item or into an allocated Jewel Socket on the Passive Skill Tree. Right click to remove from the Socket.
");

        Assert.Equal(ItemClass.AbyssJewel, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Ghastly Eye Jewel", actual.Definition.Type);

        actual.AssertDoesNotHaveModifier(StatCategory.Explicit, "Abyss");
    }
}
