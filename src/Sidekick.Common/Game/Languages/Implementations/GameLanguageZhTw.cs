namespace Sidekick.Common.Game.Languages.Implementations;

[GameLanguage("Traditional Chinese", "zh")]
public class GameLanguageZhTw : IGameLanguage
{
    public string LanguageCode => "zh";

    public string PoeTradeBaseUrl => new("http://www.pathofexile.tw/trade/");

    public string PoeTradeApiBaseUrl => new("http://www.pathofexile.tw/api/trade/");

    public string Poe2TradeBaseUrl => new("http://www.pathofexile.tw/trade2/");

    public string Poe2TradeApiBaseUrl => new("http://www.pathofexile.tw/api/trade2/");

    public Uri PoeCdnBaseUrl => new("https://web.poecdn.com/");

    public string RarityUnique => "傳奇";

    public string RarityRare => "稀有";

    public string RarityMagic => "魔法";

    public string RarityNormal => "普通";

    public string RarityCurrency => "通貨";

    public string RarityGem => "寶石";

    public string RarityDivinationCard => "命運卡";

    public string DescriptionUnidentified => "未鑑定";

    public string DescriptionQuality => "品質";

    public string DescriptionAlternateQuality => "替代品質";

    public string DescriptionCorrupted => "已汙染";

    public string DescriptionSockets => "插槽";

    public string DescriptionItemLevel => "物品等級";

    public string DescriptionExperience => "經驗值";

    public string DescriptionPhysicalDamage => "物理傷害";

    public string DescriptionElementalDamage => "元素傷害";

    public string DescriptionFireDamage => "__";

    public string DescriptionColdDamage => "__";

    public string DescriptionLightningDamage => "__";

    public string DescriptionChaosDamage => "__";

    public string DescriptionAttacksPerSecond => "每秒攻擊次數";

    public string DescriptionCriticalStrikeChance => "暴擊率";

    public string DescriptionEnergyShield => "能量護盾";

    public string DescriptionArmour => "護甲";

    public string DescriptionEvasion => "閃避值";

    public string DescriptionChanceToBlock => "格擋率";

    public string DescriptionLevel => "物品等級";

    public string DescriptionMapTier => "地圖階級";

    public string DescriptionItemQuantity => "物品數量";

    public string DescriptionItemRarity => "物品稀有度";

    public string DescriptionMonsterPackSize => "怪物群大小";

    public string DescriptionRequirements => "__TranslationRequired__:";

    public string DescriptionAreaLevel => "__TranslationRequired__:";

    public string AffixSuperior => "精良的";

    public string AffixBlighted => "凋落的";

    public string AffixBlightRavaged => "__TranslationRequired__";

    public string AffixAnomalous => "異常的";

    public string AffixDivergent => "相異的";

    public string AffixPhantasmal => "幻影的";

    public string InfluenceShaper => "塑者之物";

    public string InfluenceElder => "尊師之物";

    public string InfluenceCrusader => "聖戰軍王物品";

    public string InfluenceHunter => "狩獵者物品";

    public string InfluenceRedeemer => "救贖者物品";

    public string InfluenceWarlord => "總督軍物品";

    public ClassLanguage Classes => new()
    {
        Prefix = "___",
        DivinationCard = "___",
        StackableCurrency = "___",
        Jewel = "___",
        DelveStackableSocketableCurrency = "___",
        MetamorphSample = "___",
        HeistTool = "___",
        Amulet = "___",
        Ring = "___",
        Belt = "___",
        Gloves = "___",
        Boots = "___",
        BodyArmours = "___",
        Helmets = "___",
        Shields = "___",
        Quivers = "___",
        LifeFlasks = "___",
        ManaFlasks = "___",
        HybridFlasks = "___",
        UtilityFlasks = "___",
        ActiveSkillGems = "___",
        SupportSkillGems = "___",
        Maps = "___",
        MapFragments = "___",
        Contract = "___",
        Blueprint = "___",
        MiscMapItems = "___",
        Claws = "___",
        Daggers = "___",
        Wands = "___",
        OneHandSwords = "___",
        ThrustingOneHandSwords = "___",
        OneHandAxes = "___",
        OneHandMaces = "___",
        Bows = "___",
        Staves = "___",
        TwoHandSwords = "___",
        TwoHandAxes = "___",
        TwoHandMaces = "___",
        Sceptres = "___",
        RuneDaggers = "___",
        Warstaves = "___",
        FishingRods = "___",
        HeistGear = "___",
        HeistBrooch = "___",
        HeistTarget = "___",
        HeistCloak = "___",
        AbyssJewel = "___",
        Trinkets = "___",
        Logbooks = "___",
        MemoryLine = "___",
        SanctumResearch = "___",
        SanctumRelics = "___",
        Tinctures = "___",
        Corpses = "___",
    };
}
