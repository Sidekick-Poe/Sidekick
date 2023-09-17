using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class AscendancyParsing
    {
        private readonly IItemParser parser;

        public AscendancyParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseEnchantWithAdditionalProjectiles()
        {
            var actual = parser.ParseItem(@"Item Class: Helmets
Rarity: Rare
Corruption Crown
Regicide Mask
--------
Evasion Rating: 234 (augmented)
Energy Shield: 57 (augmented)
--------
Requirements:
Level: 63
Dex: 58
Int: 58
--------
Sockets: G
--------
Item Level: 83
--------
Split Arrow fires 2 additional Projectiles (enchant)
--------
+10 to Dexterity
+83 to Evasion Rating
+26 to maximum Energy Shield
14% increased Rarity of Items found
27% increased Stun and Block Recovery
");

            actual.AssertHasModifier(ModifierCategory.Enchant, "Split Arrow fires an additional Projectile", 2);
        }

        [Fact]
        public void TributeToTheGoddess()
        {
            var actual = parser.ParseItem(@"Item Class: Map Fragments
Rarity: Normal
Tribute to the Goddess
--------
You may appeal to the Goddess for another verdict,
but justice favours only the truly worthy.
--------
Travel to the Aspirants' Plaza and spend this item to open the Eternal Labyrinth of Fortune. You must have completed the six different Trials of Ascendancy found in Maps in order to access this area.
");

            Assert.Equal(Class.MapFragments, actual.Metadata.Class);
            Assert.Equal(Rarity.Normal, actual.Metadata.Rarity);
            Assert.Equal(Category.Map, actual.Metadata.Category);
            Assert.Equal("Tribute to the Goddess", actual.Metadata.Type);
        }
    }
}
