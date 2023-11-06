using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class MetamorphParsing
    {
        private readonly IItemParser parser;

        public MetamorphParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
        }

        [Fact]
        public void NoxiousCatalyst()
        {
            var actual = parser.ParseItem(@"Item Class: Stackable Currency
Rarity: Currency
Noxious Catalyst
--------
Stack Size: 1/10
--------
Adds quality that enhances Physical and Chaos Damage modifiers on a ring, amulet or belt
Replaces other quality types
--------
Right click this item then left click a ring, amulet or belt to apply it. Has greater effect on lower-rarity jewellery. The maximum quality is 20%.
");

            Assert.Equal(Class.StackableCurrency, actual.Header.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Noxious Catalyst", actual.Metadata.Type);
        }

        [Fact]
        public void MerveilBrain()
        {
            var actual = parser.ParseItem(@"Item Class: Metamorph Samples
Rarity: Unique
Merveil, the Returned's Brain
--------
Uses: Geyser Arena
--------
Item Level: 83
--------
Drops additional Currency Items
Drops additional Essences
--------
Combine this with four other different samples in Tane's Laboratory.
");

            Assert.Equal(Class.MetamorphSample, actual.Header.Class);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal(Category.ItemisedMonster, actual.Metadata.Category);
            Assert.Equal("Merveil, the Returned", actual.Metadata.Type);
        }

        [Fact]
        public void ParseOrgan()
        {
            var actual = parser.ParseItem(@"Item Class: Metamorph Samples
Rarity: Unique
Portentia, the Foul's Heart
--------
Uses: Blood Bubble
--------
Item Level: 79
--------
Drops a Rare Weapon
Drops additional Rare Armour
Drops additional Rare Armour
Drops additional Rare Jewellery
--------
Combine this with four other different samples in Tane's Laboratory.");

            Assert.Equal(Category.ItemisedMonster, actual.Metadata.Category);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal("Portentia, the Foul's Heart", actual.Metadata.Name);
            Assert.Equal("Portentia, the Foul", actual.Metadata.Type);
        }

        [Fact]
        public void Portentia()
        {
            var actual = parser.ParseItem(@"Item Class: Metamorph Samples
Rarity: Unique
Portentia, the Foul's Heart
--------
Uses: Blood Bubble
--------
Item Level: 79
--------
Drops additional Rare Jewellery
Drops additional Currency Items
Drops additional Currency Items
Drops additional Rare Weapons
Drops additional Rare Armour
--------
Combine this with four other different samples in Tane's Laboratory.");

            Assert.Equal(Class.MetamorphSample, actual.Header.Class);
            Assert.Equal(Category.ItemisedMonster, actual.Metadata.Category);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal("Portentia, the Foul's Heart", actual.Metadata.Name);
            Assert.Equal("Portentia, the Foul", actual.Metadata.Type);
        }

    }
}
