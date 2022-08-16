using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Items.Modifiers;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class RingParsing
    {
        private readonly IItemParser parser;

        public RingParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseBroodCircle()
        {
            var actual = parser.ParseItem(@"Item Class: Rings
Rarity: Rare
Brood Circle
Ruby Ring
--------
Requirements:
Level: 36
--------
Item Level: 76
--------
Anger has 18% increased Aura Effect (implicit)
--------
+16 to all Attributes
+31 to Intelligence
Adds 8 to 13 Physical Damage to Attacks
31% increased Mana Regeneration Rate
--------
Corrupted
");

            Assert.Equal(Class.Ring, actual.Metadata.Class);
            Assert.Equal(Category.Accessory, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Ruby Ring", actual.Metadata.Type);

            Assert.Equal(76, actual.Properties.ItemLevel);
            Assert.True(actual.Properties.Identified);
            Assert.True(actual.Properties.Corrupted);

            actual.AssertHasModifier(ModifierCategory.Implicit, "Anger has #% increased Aura Effect");
            actual.AssertHasModifier(ModifierCategory.Explicit, "# to all Attributes");
            actual.AssertHasModifier(ModifierCategory.Explicit, "# to Intelligence");
            actual.AssertHasModifier(ModifierCategory.Explicit, "Adds # to # Physical Damage to Attacks");
            actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Mana Regeneration Rate");
        }
    }
}
