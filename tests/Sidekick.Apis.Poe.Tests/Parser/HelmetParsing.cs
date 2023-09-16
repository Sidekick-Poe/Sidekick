using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class HelmetParsing
    {
        private readonly IItemParser parser;

        public HelmetParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseBlightGuardian()
        {
            var actual = parser.ParseItem(@"Item Class: Unknown
Rarity: Rare
Blight Guardian
Hunter Hood
--------
Evasion Rating: 231 (augmented)
--------
Requirements:
Level: 64
Dex: 87
--------
Sockets: G
--------
Item Level: 80
--------
Adds 28 to 51 Fire Damage to Spells
+28 to Evasion Rating
+47 to maximum Life
11% increased Rarity of Items found
+29% to Cold Resistance
You have Shocking Conflux for 3 seconds every 8 seconds
--------
Hunter Item
");

            Assert.Equal(Category.Armour, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Hunter Hood", actual.Metadata.Type);

            actual.AssertHasModifier(ModifierCategory.Explicit, "You have Shocking Conflux for 3 seconds every 8 seconds");
        }

        [Fact]
        public void ParseStarkonjaHead()
        {
            var actual = parser.ParseItem(@"Item Class: Helmets
Rarity: Unique
Starkonja's Head
Silken Hood
--------
Evasion Rating: 793 (augmented)
--------
Requirements:
Level: 60
Dex: 138
--------
Sockets: G
--------
Item Level: 63
--------
+53 to Dexterity
50% reduced Damage when on Low Life
10% increased Attack Speed
25% increased Global Critical Strike Chance
124% increased Evasion Rating
+80 to maximum Life
150% increased Global Evasion Rating when on Low Life
--------
There was no hero made out of Starkonja's death,
but merely a long sleep made eternal.
--------
Note: ~price 1 chaos
");

            Assert.Equal(Category.Armour, actual.Metadata.Category);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal("Starkonja's Head", actual.Metadata.Name);
            Assert.Equal("Silken Hood", actual.Metadata.Type);

            Assert.True(actual.Properties.Identified);
            Assert.Equal(63, actual.Properties.ItemLevel);
            Assert.Equal(793, actual.Properties.Evasion);

            actual.AssertHasModifier(ModifierCategory.Explicit, "+# to Dexterity", 53);
            actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Damage when on Low Life", 50);
            actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Attack Speed", 10);
            actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Global Critical Strike Chance", 25);
            actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Evasion Rating", 124);
            actual.AssertHasModifier(ModifierCategory.Explicit, "+# to maximum Life", 80);
            actual.AssertHasModifier(ModifierCategory.Explicit, "#% increased Global Evasion Rating when on Low Life", 150);
        }
    }
}
