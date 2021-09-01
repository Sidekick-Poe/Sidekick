using System.Linq;
using Sidekick.Apis.Poe;
using Sidekick.Common.Game.Items;
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
        public void ParseAgonyFinger()
        {
            var actual = parser.ParseItem(AgonyFinger);

            Assert.Equal(Category.Accessory, actual.Metadata.Category);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal("Amethyst Ring", actual.Metadata.Type);

            var implicits = actual.Modifiers.Implicit.Select(x => x.Text);
            Assert.Contains("Cannot be Ignited", implicits);
            Assert.Contains("Anger has 20% increased Aura Effect", implicits);
        }

        #region ItemText

        private const string AgonyFinger = @"Item Class: Unknown
Rarity: Rare
Agony Finger
Amethyst Ring
--------
Requirements:
Level: 59
--------
Item Level: 77
--------
Cannot be Ignited (implicit)
Anger has 20% increased Aura Effect (implicit)
--------
+49 to Dexterity
+78 to Evasion Rating
+35 to maximum Mana
+31% to Lightning Resistance
+7% to Chaos Resistance
0.39% of Physical Attack Damage Leeched as Life
--------
Corrupted
";

        #endregion
    }
}
