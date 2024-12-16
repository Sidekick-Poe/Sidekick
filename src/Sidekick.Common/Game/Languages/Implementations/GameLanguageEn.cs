namespace Sidekick.Common.Game.Languages.Implementations;

[GameLanguage("English", "en")]
public class GameLanguageEN : IGameLanguage
{
    public string PoeTradeBaseUrl => "https://www.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://www.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://www.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://www.pathofexile.com/api/trade2/";
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
    public string DescriptionFireDamage => "Fire Damage";
    public string DescriptionColdDamage => "Cold Damage";
    public string DescriptionLightningDamage => "Lightning Damage";
    public string DescriptionChaosDamage => "Chaos Damage";
    public string DescriptionEnergyShield => "Energy Shield";
    public string DescriptionArmour => "Armour";
    public string DescriptionEvasion => "Evasion Rating";
    public string DescriptionChanceToBlock => "Chance to Block";
    public string DescriptionAttacksPerSecond => "Attacks per Second";
    public string DescriptionCriticalStrikeChance => "Critical Strike Chance";
    public string DescriptionMapTier => "Map Tier";
    public string DescriptionItemQuantity => "Item Quantity";
    public string DescriptionItemRarity => "Item Rarity";
    public string DescriptionMonsterPackSize => "Monster Pack Size";
    public string DescriptionRequirements => "Requirements";
    public string DescriptionAreaLevel => "Area Level";

    public string AffixSuperior => "Superior";
    public string AffixBlighted => "Blighted";
    public string AffixBlightRavaged => "Blight-ravaged";
    public string AffixAnomalous => "Anomalous";
    public string AffixDivergent => "Divergent";
    public string AffixPhantasmal => "Phantasmal";

    public string InfluenceShaper => "Shaper Item";
    public string InfluenceElder => "Elder Item";
    public string InfluenceCrusader => "Crusader Item";
    public string InfluenceHunter => "Hunter Item";
    public string InfluenceRedeemer => "Redeemer Item";
    public string InfluenceWarlord => "Warlord Item";

    public ClassLanguage Classes { get; } = new()
    {
        Prefix = "Item Class",
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
        ActiveSkillGems = "Skill Gems",
        SupportSkillGems = "Support Gems",
        Maps = "Maps",
        MapFragments = "Map Fragments",
        Contract = "Contracts",
        Blueprint = "Blueprints",
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
        Logbooks = "Expedition Logbooks",
        MemoryLine = "Memories",
        SanctumRelics = "Relics",
        Tinctures = "Tinctures",
        Corpses = "Corpses",
        SanctumResearch = "Sanctum Research",
    };
}

