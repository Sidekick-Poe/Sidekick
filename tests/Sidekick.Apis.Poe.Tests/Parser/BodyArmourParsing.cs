using System.Linq;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Items.Modifiers;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class BodyArmourParsing
    {
        private readonly IItemParser parser;

        public BodyArmourParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseSixLinkUniqueBodyArmor()
        {
            var actual = parser.ParseItem(@"Item Class: Unknown
Rarity: Unique
Carcass Jack
Varnished Coat
--------
Quality: +20% (augmented)
Evasion Rating: 960 (augmented)
Energy Shield: 186 (augmented)
--------
Requirements:
Level: 70
Str: 68
Dex: 96
Int: 111
--------
Sockets: B-B-R-B-B-B
--------
Item Level: 81
--------
128% increased Evasion and Energy Shield
+55 to maximum Life
+12% to all Elemental Resistances
44% increased Area of Effect
47% increased Area Damage
Extra gore
--------
""...The discomfort shown by the others is amusing, but none
can deny that my work has made quite the splash...""
- Maligaro's Journal
");

            Assert.Equal(Category.Armour, actual.Metadata.Category);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal("Carcass Jack", actual.Metadata.Name);
            Assert.Equal("Varnished Coat", actual.Metadata.Type);
            Assert.Equal(20, actual.Properties.Quality);
            Assert.Equal(960, actual.Properties.Evasion);
            Assert.Equal(186, actual.Properties.EnergyShield);
            Assert.Equal(6, actual.Sockets.Count(x => x.Group == 0));

            actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Evasion and Energy Shield (Local)", 128);
            actual.AssertHasModifier(ModifierCategory.Explicit, "+# to maximum Life", 55);
            actual.AssertHasModifier(ModifierCategory.Explicit, "+#% to all Elemental Resistances", 12);
            actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Area of Effect", 44);
            actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Area Damage", 47);
            actual.AssertHasModifier(ModifierCategory.Explicit, "Extra gore");

            actual.AssertHasPseudoModifier("+12% total to all Elemental Resistances", 12);
            actual.AssertHasPseudoModifier("+36% total Elemental Resistance", 36);
            actual.AssertHasPseudoModifier("+36% total Resistance", 36);
            actual.AssertHasPseudoModifier("+55 total maximum Life", 55);
        }

        [Fact]
        public void DaressosDefiance()
        {
            var actual = parser.ParseItem(@"Item Class: Body Armours
Rarity: Unique
Daresso's Defiance
Full Dragonscale
--------
Armour: 1260 (augmented)
Evasion Rating: 1000 (augmented)
--------
Requirements:
Level: 63
Str: 115
Dex: 94
--------
Sockets: G-R-G
--------
Item Level: 74
--------
168% increased Armour and Evasion
+65 to maximum Life
0.55% of Physical Attack Damage Leeched as Life
6% chance to Dodge Attack Hits
You lose all Endurance Charges when Hit
You gain an Endurance Charge on Kill
You gain Onslaught for 5 seconds per Endurance Charge when Hit
67% increased Onslaught Effect
--------
""When your back is against the wall,
And your opponent is bleeding you dry,
There is only one appropriate response:
Unbridled, overwhelming violence.""
- Daresso, the Sword King
--------
Note: ~price 2 chaos
");

            Assert.Equal(Class.BodyArmours, actual.Metadata.Class);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal(Category.Armour, actual.Metadata.Category);
            Assert.Equal("Daresso's Defiance", actual.Metadata.Name);
            Assert.Equal("Full Dragonscale", actual.Metadata.Type);
        }
    }
}
