using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1.Parser
{
    [Collection(Collections.Poe1Parser)]
    public class RingParsing(ParserFixture fixture)
    {
        private readonly IItemParser parser = fixture.Parser;

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

            Assert.Equal("accessory.ring", actual.Header.ItemCategory);
            Assert.Equal(Category.Accessory, actual.Header.Category);
            Assert.Equal(Rarity.Rare, actual.Header.Rarity);
            Assert.Equal("Ruby Ring", actual.Header.ApiType);

            Assert.Equal(76, actual.Properties.ItemLevel);
            Assert.True(actual.Properties.Unidentified);
            Assert.True(actual.Properties.Corrupted);

            actual.AssertHasModifier(ModifierCategory.Implicit, "Anger has #% increased Aura Effect", 18);
            actual.AssertHasModifier(ModifierCategory.Explicit, "+# to all Attributes", 16);
            actual.AssertHasModifier(ModifierCategory.Explicit, "+# to Intelligence", 31);
            actual.AssertHasModifier(ModifierCategory.Explicit, "Adds # to # Physical Damage to Attacks", 8, 13);
            actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Mana Regeneration Rate", 31);
        }

        [Fact]
        public void ParseBerekGrip()
        {
            var actual = parser.ParseItem(@"Item Class: Rings
Rarity: Unique
Berek's Grip
Two-Stone Ring
--------
Requirements:
Level: 20
--------
Item Level: 84
--------
+13% to Cold and Lightning Resistances (implicit)
--------
28% increased Cold Damage
Adds 1 to 67 Lightning Damage to Spells and Attacks
+30 to maximum Life
1% of Damage Leeched as Life against Shocked Enemies
1% of Damage Leeched as Energy Shield against Frozen Enemies
--------
""Berek hid from Storm's lightning wrath
In the embrace of oblivious Frost
Repelled by ice, blinded by blizzards
Storm raged in vain
While Berek slept.""
- Berek and the Untamed
");

            Assert.True(actual.Properties.Unidentified);
        }
    }
}
