using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class AfflictionParsing
    {
        private readonly IItemParser parser;

        public AfflictionParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void BulbonicTrail()
        {
            var actual = parser.ParseItem(@"Item Class: Charms
Rarity: Magic
Corvine Charm of the Trickster
--------
Item Level: 70
--------
Recover 2% of Life on Kill
Recover 2% of Energy Shield on Kill
Recover 2% of Mana on Kill
--------
Place into an allocated Charm Socket on the Wildwood Primalist Passive Skill Tree. Right click to remove from the Socket.
--------
Unmodifiable
");

            Assert.Equal(Class.AfflictionCharms, actual.Header.Class);
            Assert.Equal(Category.Affliction, actual.Metadata.Category);
            Assert.Equal(Rarity.Magic, actual.Metadata.Rarity);
            Assert.Equal("Corvine Charm", actual.Metadata.Type);
        }
    }
}
