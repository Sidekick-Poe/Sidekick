using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class NecropolisParsing
    {
        private readonly IItemParser parser;

        public NecropolisParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void AllflameEmberOfHinekora()
        {
            var actual = parser.ParseItem(@"Item Class: Embers of the Allflame
Allflame Ember of Hinekora
--------
Pack Size: 5-7 Monsters
Pack contains Pack Leader
--------
Item Level: 69
--------
Pack Monsters can drop Karui Tattoos (implicit)
--------
Click and drag this item over a Monster Pack in the Lantern of Arimor to have those packs in the area replaced with this pack.
");

            Assert.Equal(Class.EmbersOfTheAllflame, actual.Header.Class);
            Assert.Equal(Rarity.Unknown, actual.Metadata.Rarity);
            Assert.Equal(Category.EmbersOfTheAllflame, actual.Metadata.Category);
            Assert.Equal("Allflame Ember of Hinekora", actual.Metadata.Type);
        }

    }
}
