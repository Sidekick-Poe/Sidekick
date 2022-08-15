using System.Linq;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class AbyssParsing
    {
        private readonly IItemParser parser;

        public AbyssParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseBulbonicTrail()
        {
            var actual = parser.ParseItem(@"Item Class: Boots
Rarity: Unique
Bubonic Trail
Murder Boots
--------
Evasion Rating: 185
Energy Shield: 17
--------
Requirements:
Level: 69
Dex: 82
Int: 42
--------
Sockets: G-G A
--------
Item Level: 84
--------
Has 1 Abyssal Socket
Triggers Level 20 Death Walk when Equipped
6% increased maximum Life
30% increased Movement Speed
10% increased Damage for each type of Abyss Jewel affecting you
--------
Even the dead serve the Lightless.
");

            Assert.Equal(Class.Boots, actual.Metadata.Class);
            Assert.Equal(Category.Armour, actual.Metadata.Category);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal("Bubonic Trail", actual.Metadata.Name);
            Assert.Equal("Murder Boots", actual.Metadata.Type);

            var modifiers = actual.ModifierLines.Select(x => x.Modifier?.Text);
            Assert.Contains("Has 1 Abyssal Socket", modifiers);
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

            Assert.Equal(Class.AbyssJewel, actual.Metadata.Class);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal(Category.Jewel, actual.Metadata.Category);
            Assert.Equal("Hypnotic Eye Jewel", actual.Metadata.Type);
        }
    }
}
