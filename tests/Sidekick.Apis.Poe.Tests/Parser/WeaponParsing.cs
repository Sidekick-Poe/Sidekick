using System.Linq;
using Sidekick.Apis.Poe;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class WeaponParsing
    {
        private readonly IItemParser parser;

        public WeaponParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseTriggerWeapon()
        {
            var actual = parser.ParseItem(@"Item Class: Wands
Rarity: Rare
Hypnotic Bite
Imbued Wand
--------
Wand
Quality: +14% (augmented)
Physical Damage: 45-82 (augmented)
Elemental Damage: 51-83 (augmented)
Critical Strike Chance: 7.00%
Attacks per Second: 1.50
--------
Requirements:
Level: 60
Int: 188
--------
Sockets: B-B-B 
--------
Item Level: 72
--------
37% increased Spell Damage (implicit)
--------
41% increased Physical Damage
81% increased Fire Damage
Adds 51 to 83 Cold Damage
+88 to Accuracy Rating
Trigger a Socketed Spell when you Use a Skill, with a 8 second Cooldown and 150% more Cost (crafted)
--------
Note: ~price 5 chaos
");

            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Imbued Wand", actual.Metadata.Type);

            var crafteds = actual.Modifiers.Crafted.Select(x => x.Text);
            Assert.Contains("Trigger a Socketed Spell when you Use a Skill, with a 8 second Cooldown and 150% more Cost", crafteds);
        }
    }
}
