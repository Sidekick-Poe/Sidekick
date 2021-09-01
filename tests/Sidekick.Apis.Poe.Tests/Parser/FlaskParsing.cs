using System.Linq;
using Sidekick.Apis.Poe;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class FlaskParsing
    {
        private readonly IItemParser parser;

        public FlaskParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void ParseSanctifiedManaFlask()
        {
            var actual = parser.ParseItem(@"Item Class: Mana Flasks
Rarity: Magic
Sanctified Mana Flask of Staunching
--------
Quality: +7% (augmented)
Recovers 1177 (augmented) Mana over 6.50 Seconds
Consumes 7 of 35 Charges on use
Currently has 0 Charges
--------
Requirements:
Level: 50
--------
Item Level: 72
--------
Grants Immunity to Bleeding for 4 seconds if used while Bleeding
Grants Immunity to Corrupted Blood for 4 seconds if used while affected by Corrupted Blood
--------
Right click to drink. Can only hold charges while in belt. Refills as you kill monsters.
");

            Assert.Equal(Category.Flask, actual.Metadata.Category);
            Assert.Equal(Rarity.Magic, actual.Metadata.Rarity);
            Assert.Equal("Sanctified Mana Flask", actual.Metadata.Type);

            var explicits = actual.Modifiers.Explicit.Select(x => x.Text);
            Assert.Contains("Grants Immunity to Bleeding for 4 seconds if used while Bleeding\nGrants Immunity to Corrupted Blood for 4 seconds if used while affected by Corrupted Blood", explicits);
        }
    }
}
