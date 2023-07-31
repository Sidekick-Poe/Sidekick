using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Items.Modifiers;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class JewelParsing
    {
        private readonly IItemParser parser;

        public JewelParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

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

            Assert.Equal(Category.Jewel, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Cobalt Jewel", actual.Metadata.Type);
            Assert.Equal("Blight Cut", actual.Original.Name);
            Assert.Equal(68, actual.Properties.ItemLevel);

            actual.AssertHasModifier(ModifierCategory.Explicit, "# to Strength and Intelligence", 8);
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

            Assert.Equal(Category.Jewel, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Large Cluster Jewel", actual.Metadata.Type);
            Assert.Equal("Chimeric Sliver", actual.Original.Name);
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

            Assert.Equal(Class.Jewel, actual.Metadata.Class);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal(Category.Jewel, actual.Metadata.Category);
            Assert.Equal("Viridian Jewel", actual.Metadata.Type);
        }
    }
}
