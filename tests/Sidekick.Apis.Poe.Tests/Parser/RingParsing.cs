using Sidekick.Common.Blazor;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Xml;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Items.Modifiers;
using Xunit;
using static MudBlazor.CategoryTypes;
using static System.Formats.Asn1.AsnWriter;

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
            actual.AssertHasModifier(ModifierCategory.Explicit, "+# to all Attributes");
            actual.AssertHasModifier(ModifierCategory.Explicit, "+# to Intelligence");
            actual.AssertHasModifier(ModifierCategory.Explicit, "Adds # to # Physical Damage to Attacks");
            actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Mana Regeneration Rate");
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

            Assert.True(actual.Properties.Identified);
        }
    }
}
