using System;

namespace Sidekick.Common.Game.Languages.Implementations
{
    [GameLanguage("English", "en")]
    public class GameLanguageEN : IGameLanguage
    {
        public string LanguageCode => "en";
        public Uri PoeTradeSearchBaseUrl => new("https://www.pathofexile.com/trade/search/");
        public Uri PoeTradeExchangeBaseUrl => new("https://www.pathofexile.com/trade/exchange/");
        public Uri PoeTradeApiBaseUrl => new("https://www.pathofexile.com/api/trade/");
        public Uri PoeCdnBaseUrl => new("https://web.poecdn.com/");
        public string RarityUnique => "Unique";
        public string RarityRare => "Rare";
        public string RarityMagic => "Magic";
        public string RarityNormal => "Normal";
        public string RarityCurrency => "Currency";
        public string RarityGem => "Gem";
        public string RarityDivinationCard => "Divination Card";
        public string DescriptionUnidentified => "Unidentified";
        public string DescriptionQuality => "Quality";
        public string DescriptionAlternateQuality => "Alternate Quality";
        public string DescriptionLevel => "Level";
        public string DescriptionCorrupted => "Corrupted";
        public string DescriptionSockets => "Sockets";
        public string DescriptionItemLevel => "Item Level";
        public string DescriptionExperience => "Experience";
        public string DescriptionPhysicalDamage => "Physical Damage";
        public string DescriptionElementalDamage => "Elemental Damage";
        public string DescriptionEnergyShield => "Energy Shield";
        public string DescriptionArmour => "Armour";
        public string DescriptionEvasion => "Evasion Rating";
        public string DescriptionChanceToBlock => "Chance to Block";
        public string DescriptionAttacksPerSecond => "Attacks per Second";
        public string DescriptionCriticalStrikeChance => "Critical Strike Chance";
        public string PrefixSuperior => "Superior";
        public string InfluenceShaper => "Shaper";
        public string InfluenceElder => "Elder";
        public string InfluenceCrusader => "Crusader";
        public string InfluenceHunter => "Hunter";
        public string InfluenceRedeemer => "Redeemer";
        public string InfluenceWarlord => "Warlord";
        public string DescriptionMapTier => "Map Tier";
        public string DescriptionItemQuantity => "Item Quantity";
        public string DescriptionItemRarity => "Item Rarity";
        public string DescriptionMonsterPackSize => "Monster Pack Size";
        public string PrefixBlighted => "Blighted";

        public string PrefixAnomalous => "Anomalous";
        public string PrefixDivergent => "Divergent";
        public string PrefixPhantasmal => "Phantasmal";

        public ClassLanguage Classes { get; } = new ClassLanguage()
        {
            Prefix = "Item Class: ",
            DivinationCard = "Divination Cards",
            StackableCurrency = "Stackable Currency",
            Jewel = "Jewels",
            DelveStackableSocketableCurrency = "Delve Stackable Socketable Currency",
            MetamorphSample = "Metamorph Samples",
            HeistTool = "Heist Tools",
            Amulet = "Amulets",
            Ring = "Rings",
            Belt = "Belts",
            Gloves = "Gloves",
            Boots = "Boots",
            BodyArmours = "Body Armours",
            Helmets = "Helmets",
            Shields = "Shields",
            Quivers = "Quivers",
            LifeFlasks = "Life Flasks",
            ManaFlasks = "Mana Flasks",
            HybridFlasks = "Hybrid Flasks",
            UtilityFlasks = "Utility Flasks",
            CriticalUtilityFlasks = "Critical Utility Flasks",
            ActiveSkillGems = "Active Skill Gems",
            SupportSkillGems = "Support Skill Gems",
            Maps = "Maps",
            MapFragments = "Map Fragments",
            Contract = "Contract",
            Blueprint = "Blueprint",
            MiscMapItems = "Misc Map Items",
            Claws = "Claws",
            Daggers = "Daggers",
            Wands = "Wands",
            OneHandSwords = "One Hand Swords",
            ThrustingOneHandSwords = "Thrusting One Hand Swords",
            OneHandAxes = "One Hand Axes",
            OneHandMaces = "One Hand Maces",
            Bows = "Bows",
            Staves = "Staves",
            TwoHandSwords = "Two Hand Swords",
            TwoHandAxes = "Two Hand Axes",
            TwoHandMaces = "Two Hand Maces",
            Sceptres = "Sceptres",
            RuneDaggers = "Rune Daggers",
            Warstaves = "Warstaves",
            FishingRods = "Fishing Rods",
            HeistGear = "Heist Gear",
            HeistBrooch = "Heist Brooches",
            HeistTarget = "Heist Targets",
            HeistCloak = "Heist Cloaks",
            AbyssJewel = "Abyss Jewels",
            Trinkets = "Trinkets",
        };
    }
}
