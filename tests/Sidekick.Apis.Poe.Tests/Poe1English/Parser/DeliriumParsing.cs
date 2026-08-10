using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Stats;
using Xunit;
namespace Sidekick.Apis.Poe.Tests.Poe1English.Parser;

[Collection(Collections.Poe1EnglishFixture)]
public class DeliriumParsing(Poe1EnglishFixture fixture)
{
    private readonly ItemParser parser = fixture.Parser;

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

        Assert.Equal(ItemClass.Unknown, actual.ItemClass.Type);
        Assert.Equal(Rarity.Currency, actual.Properties.Rarity);
        Assert.Equal("Simulacrum Splinter", actual.TradeItem?.Type);
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

        Assert.Equal(ItemClass.Jewel, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Small Cluster Jewel", actual.TradeItem?.Type);

        fixture.AssertHasStat(actual, StatCategory.Enchant, "Adds # Passive Skills", 2);
        fixture.AssertHasStat(actual, StatCategory.Enchant, "Added Small Passive Skills grant: 15% increased Evasion Rating");
    }

    [Fact]
    public void FoeEye()
    {
        var actual = parser.ParseItem(@"Item Class: Jewels
Rarity: Rare
Foe Eye
Large Cluster Jewel
--------
Requirements:
Level: 54
--------
Item Level: 70
--------
Adds 8 Passive Skills (enchant)
(Added Passive Skills are never considered to be in Radius by other Jewels) (enchant)
(All Added Passive Skills are Small unless otherwise specified) (enchant)
2 Added Passive Skills are Jewel Sockets (enchant)
Added Small Passive Skills grant: 12% increased Physical Damage (enchant)
(Passive Skills that are not Notable, Masteries, Keystones, or Jewel Sockets are Small) (enchant)
--------
{ Prefix Modifier ""Shining"" (Tier: 3) — Defences, Energy Shield }
Added Small Passive Skills also grant: +5(4-5) to Maximum Energy Shield
{ Suffix Modifier ""of Joy"" (Tier: 2) — Mana }
Added Small Passive Skills also grant: 5% increased Mana Regeneration Rate
{ Suffix Modifier ""of the Mongoose"" (Tier: 3) — Attribute }
Added Small Passive Skills also grant: +2(2-3) to Dexterity
--------
Place into an allocated Large Jewel Socket on the Passive Skill Tree. Added passives do not interact with jewel radiuses. Right click to remove from the Socket.
");

        Assert.Equal(ItemClass.Jewel, actual.ItemClass.Type);
        Assert.Equal(Rarity.Rare, actual.Properties.Rarity);
        Assert.Equal("Large Cluster Jewel", actual.TradeItem?.Type);

        fixture.AssertHasStat(actual, StatCategory.Enchant, "Adds # Passive Skills", 8);
        fixture.AssertHasStat(actual, StatCategory.Enchant, "Added Small Passive Skills grant: 12% increased Physical Damage");
    }

}
