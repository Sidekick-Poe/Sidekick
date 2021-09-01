using Sidekick.Apis.Poe;
using Sidekick.Common.Game.Items;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Parser
{
    [Collection(Collections.Mediator)]
    public class MetadataParsing
    {
        private readonly IItemParser parser;
        private readonly ItemTexts texts;

        public MetadataParsing(ParserFixture fixture)
        {
            parser = fixture.Parser;
            texts = fixture.Texts;
        }

        [Fact]
        public void ChaosOrb()
        {
            var actual = parser.ParseItem(texts.ChaosOrb);

            Assert.Equal(Class.StackableCurrency, actual.Metadata.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Chaos Orb", actual.Metadata.Type);
        }

        [Fact]
        public void ArcardeMap()
        {
            var actual = parser.ParseItem(texts.ArcardeMap);

            Assert.Equal(Class.Maps, actual.Metadata.Class);
            Assert.Equal(Rarity.Normal, actual.Metadata.Rarity);
            Assert.Equal(Category.Map, actual.Metadata.Category);
            Assert.Equal("Arcade Map", actual.Metadata.Type);
        }

        [Fact]
        public void ScreamingEssenceOfWoe()
        {
            var actual = parser.ParseItem(texts.ScreamingEssenceOfWoe);

            Assert.Equal(Class.StackableCurrency, actual.Metadata.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Screaming Essence of Woe", actual.Metadata.Type);
        }

        [Fact]
        public void SacrificeAtMidnight()
        {
            var actual = parser.ParseItem(texts.SacrificeAtMidnight);

            Assert.Equal(Class.MapFragments, actual.Metadata.Class);
            Assert.Equal(Rarity.Normal, actual.Metadata.Rarity);
            Assert.Equal(Category.Map, actual.Metadata.Category);
            Assert.Equal("Sacrifice at Midnight", actual.Metadata.Type);
        }

        [Fact]
        public void RitualSplinter()
        {
            var actual = parser.ParseItem(texts.RitualSplinter);

            Assert.Equal(Class.StackableCurrency, actual.Metadata.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Ritual Splinter", actual.Metadata.Type);
        }

        [Fact]
        public void TimelessEternalEmpireSplinter()
        {
            var actual = parser.ParseItem(texts.TimelessEternalEmpireSplinter);

            Assert.Equal(Class.StackableCurrency, actual.Metadata.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Timeless Eternal Empire Splinter", actual.Metadata.Type);
        }

        [Fact]
        public void DivineVessel()
        {
            var actual = parser.ParseItem(texts.DivineVessel);

            Assert.Equal(Class.MapFragments, actual.Metadata.Class);
            Assert.Equal(Rarity.Normal, actual.Metadata.Rarity);
            Assert.Equal(Category.Map, actual.Metadata.Category);
            Assert.Equal("Divine Vessel", actual.Metadata.Type);
        }

        [Fact]
        public void TributeToTheGoddess()
        {
            var actual = parser.ParseItem(texts.TributeToTheGoddess);

            Assert.Equal(Class.MapFragments, actual.Metadata.Class);
            Assert.Equal(Rarity.Normal, actual.Metadata.Rarity);
            Assert.Equal(Category.Map, actual.Metadata.Category);
            Assert.Equal("Tribute to the Goddess", actual.Metadata.Type);
        }

        [Fact]
        public void SplinterOfTul()
        {
            var actual = parser.ParseItem(texts.SplinterOfTul);

            Assert.Equal(Class.StackableCurrency, actual.Metadata.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Splinter of Tul", actual.Metadata.Type);
        }

        [Fact]
        public void RustedReliquaryScarab()
        {
            var actual = parser.ParseItem(texts.RustedReliquaryScarab);

            Assert.Equal(Class.MapFragments, actual.Metadata.Class);
            Assert.Equal(Rarity.Normal, actual.Metadata.Rarity);
            Assert.Equal(Category.Map, actual.Metadata.Category);
            Assert.Equal("Rusted Reliquary Scarab", actual.Metadata.Type);
        }

        [Fact]
        public void BoonOfJustice()
        {
            var actual = parser.ParseItem(texts.BoonOfJustice);

            Assert.Equal(Class.DivinationCard, actual.Metadata.Class);
            Assert.Equal(Rarity.DivinationCard, actual.Metadata.Rarity);
            Assert.Equal(Category.DivinationCard, actual.Metadata.Category);
            Assert.Equal("Boon of Justice", actual.Metadata.Type);
        }

        [Fact]
        public void DaressosDefiance()
        {
            var actual = parser.ParseItem(texts.DaressosDefiance);

            Assert.Equal(Class.BodyArmours, actual.Metadata.Class);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal(Category.Armour, actual.Metadata.Category);
            Assert.Equal("Daresso's Defiance", actual.Metadata.Name);
            Assert.Equal("Full Dragonscale", actual.Metadata.Type);
        }

        [Fact]
        public void ClearOil()
        {
            var actual = parser.ParseItem(texts.ClearOil);

            Assert.Equal(Class.StackableCurrency, actual.Metadata.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Clear Oil", actual.Metadata.Type);
        }

        [Fact]
        public void BlightedSpiderLairMap()
        {
            var actual = parser.ParseItem(texts.BlightedSpiderLairMap);

            Assert.Equal(Class.Maps, actual.Metadata.Class);
            Assert.Equal(Rarity.Normal, actual.Metadata.Rarity);
            Assert.Equal(Category.Map, actual.Metadata.Category);
            Assert.Equal("Spider Lair Map", actual.Metadata.Type);
        }

        [Fact]
        public void SimulacrumSplinter()
        {
            var actual = parser.ParseItem(texts.SimulacrumSplinter);

            Assert.Equal(Class.StackableCurrency, actual.Metadata.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Simulacrum Splinter", actual.Metadata.Type);
        }

        [Fact]
        public void PerfectFossil()
        {
            var actual = parser.ParseItem(texts.PerfectFossil);

            Assert.Equal(Class.StackableCurrency, actual.Metadata.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Perfect Fossil", actual.Metadata.Type);
        }

        [Fact]
        public void PowerfulChaoticResonator()
        {
            var actual = parser.ParseItem(texts.PowerfulChaoticResonator);

            Assert.Equal(Class.DelveStackableSocketableCurrency, actual.Metadata.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Powerful Chaotic Resonator", actual.Metadata.Type);
        }

        [Fact]
        public void NoxiousCatalyst()
        {
            var actual = parser.ParseItem(texts.NoxiousCatalyst);

            Assert.Equal(Class.StackableCurrency, actual.Metadata.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Noxious Catalyst", actual.Metadata.Type);
        }

        [Fact]
        public void BloodProgenitorsBrain()
        {
            var actual = parser.ParseItem(texts.BloodProgenitorsBrain);

            Assert.Equal(Class.MetamorphSample, actual.Metadata.Class);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal(Category.ItemisedMonster, actual.Metadata.Category);
            Assert.Equal("Blood Progenitor", actual.Metadata.Type);
        }

        [Fact]
        public void HallowedLifeFlask()
        {
            var actual = parser.ParseItem(texts.HallowedLifeFlask);

            Assert.Equal(Class.LifeFlasks, actual.Metadata.Class);
            Assert.Equal(Rarity.Normal, actual.Metadata.Rarity);
            Assert.Equal(Category.Flask, actual.Metadata.Category);
            Assert.Equal("Hallowed Life Flask", actual.Metadata.Type);
        }

        [Fact]
        public void HallowedManaFlask()
        {
            var actual = parser.ParseItem(texts.HallowedManaFlask);

            Assert.Equal(Class.ManaFlasks, actual.Metadata.Class);
            Assert.Equal(Rarity.Normal, actual.Metadata.Rarity);
            Assert.Equal(Category.Flask, actual.Metadata.Category);
            Assert.Equal("Hallowed Mana Flask", actual.Metadata.Type);
        }

        [Fact]
        public void ArcaneSurgeSupport()
        {
            var actual = parser.ParseItem(texts.ArcaneSurgeSupport);

            Assert.Equal(Class.SupportSkillGems, actual.Metadata.Class);
            Assert.Equal(Rarity.Gem, actual.Metadata.Rarity);
            Assert.Equal(Category.Gem, actual.Metadata.Category);
            Assert.Equal("Arcane Surge Support", actual.Metadata.Type);
        }

        [Fact]
        public void VoidSphere()
        {
            var actual = parser.ParseItem(texts.VoidSphere);

            Assert.Equal(Class.ActiveSkillGems, actual.Metadata.Class);
            Assert.Equal(Rarity.Gem, actual.Metadata.Rarity);
            Assert.Equal(Category.Gem, actual.Metadata.Category);
            Assert.Equal("Void Sphere", actual.Metadata.Type);
        }

        [Fact]
        public void SacredHybridFlask()
        {
            var actual = parser.ParseItem(texts.SacredHybridFlask);

            Assert.Equal(Class.HybridFlasks, actual.Metadata.Class);
            Assert.Equal(Rarity.Normal, actual.Metadata.Rarity);
            Assert.Equal(Category.Flask, actual.Metadata.Category);
            Assert.Equal("Sacred Hybrid Flask", actual.Metadata.Type);
        }

        [Fact]
        public void ViridianJewel()
        {
            var actual = parser.ParseItem(texts.ViridianJewel);

            Assert.Equal(Class.Jewel, actual.Metadata.Class);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal(Category.Jewel, actual.Metadata.Category);
            Assert.Equal("Viridian Jewel", actual.Metadata.Type);
        }

        [Fact]
        public void SmallClusterJewel()
        {
            var actual = parser.ParseItem(texts.SmallClusterJewel);

            Assert.Equal(Class.Jewel, actual.Metadata.Class);
            Assert.Equal(Rarity.Magic, actual.Metadata.Rarity);
            Assert.Equal(Category.Jewel, actual.Metadata.Category);
            Assert.Equal("Small Cluster Jewel", actual.Metadata.Type);
        }

        [Fact]
        public void HeistTool()
        {
            var actual = parser.ParseItem(texts.HeistTool);

            Assert.Equal(Class.HeistTool, actual.Metadata.Class);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal(Category.HeistEquipment, actual.Metadata.Category);
            Assert.Equal("Lustrous Ward", actual.Metadata.Type);
        }

        [Fact]
        public void InscribedUltimatum()
        {
            var actual = parser.ParseItem(texts.InscribedUltimatum);

            Assert.Equal(Class.MiscMapItems, actual.Metadata.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Map, actual.Metadata.Category);
            Assert.Equal("Inscribed Ultimatum", actual.Metadata.Type);
        }

        [Fact]
        public void Reefbane()
        {
            var actual = parser.ParseItem(texts.Reefbane);

            Assert.Equal(Class.FishingRods, actual.Metadata.Class);
            Assert.Equal(Rarity.Unique, actual.Metadata.Rarity);
            Assert.Equal(Category.Weapon, actual.Metadata.Category);
            Assert.Equal("Reefbane", actual.Metadata.Name);
            Assert.Equal("Fishing Rod", actual.Metadata.Type);
        }

        [Fact]
        public void FarricChieftain()
        {
            var actual = parser.ParseItem(texts.FarricChieftain);

            Assert.Equal(Class.StackableCurrency, actual.Metadata.Class);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal(Category.ItemisedMonster, actual.Metadata.Category);
            Assert.Equal("Farric Chieftain", actual.Metadata.Type);
        }

        [Fact]
        public void HeistCloak()
        {
            var actual = parser.ParseItem(texts.HeistCloak);

            Assert.Equal(Class.HeistCloak, actual.Metadata.Class);
            Assert.Equal(Rarity.Magic, actual.Metadata.Rarity);
            Assert.Equal(Category.HeistEquipment, actual.Metadata.Category);
            Assert.Equal("Torn Cloak", actual.Metadata.Type);
        }

        [Fact]
        public void HeistBrooch()
        {
            var actual = parser.ParseItem(texts.HeistBrooch);

            Assert.Equal(Class.HeistBrooch, actual.Metadata.Class);
            Assert.Equal(Rarity.Normal, actual.Metadata.Rarity);
            Assert.Equal(Category.HeistEquipment, actual.Metadata.Category);
            Assert.Equal("Silver Brooch", actual.Metadata.Type);
        }

        [Fact]
        public void HeistGear()
        {
            var actual = parser.ParseItem(texts.HeistGear);

            Assert.Equal(Class.HeistGear, actual.Metadata.Class);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal(Category.HeistEquipment, actual.Metadata.Category);
            Assert.Equal("Rough Sharpening Stone", actual.Metadata.Type);
        }

        [Fact]
        public void HeistTarget()
        {
            var actual = parser.ParseItem(texts.HeistTarget);

            Assert.Equal(Class.HeistTarget, actual.Metadata.Class);
            Assert.Equal(Rarity.Currency, actual.Metadata.Rarity);
            Assert.Equal(Category.Currency, actual.Metadata.Category);
            Assert.Equal("Alchemical Chalice", actual.Metadata.Type);
        }

        [Fact]
        public void ThiefTrinket()
        {
            var actual = parser.ParseItem(texts.ThiefTrinket);

            Assert.Equal(Class.Trinkets, actual.Metadata.Class);
            Assert.Equal(Rarity.Rare, actual.Metadata.Rarity);
            Assert.Equal(Category.Accessory, actual.Metadata.Category);
            Assert.Equal("Thief's Trinket", actual.Metadata.Type);
        }

        [Fact]
        public void AbyssJewel()
        {
            var actual = parser.ParseItem(texts.AbyssJewel);

            Assert.Equal(Class.AbyssJewel, actual.Metadata.Class);
            Assert.Equal(Rarity.Magic, actual.Metadata.Rarity);
            Assert.Equal(Category.Jewel, actual.Metadata.Category);
            Assert.Equal("Hypnotic Eye Jewel", actual.Metadata.Type);
        }
    }
}
