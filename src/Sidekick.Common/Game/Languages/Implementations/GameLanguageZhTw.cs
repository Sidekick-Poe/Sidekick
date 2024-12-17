namespace Sidekick.Common.Game.Languages.Implementations;

[GameLanguage("Traditional Chinese", "zh")]
public class GameLanguageZhTw : IGameLanguage
{
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

    public string DescriptionFireDamage => "火焰傷害";

    public string DescriptionColdDamage => "冰冷傷害";

    public string DescriptionLightningDamage => "閃電傷害";

    public string DescriptionChaosDamage => "混沌傷害";

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

    public string DescriptionRequirements => "需求";

    public string DescriptionAreaLevel => "區域等級";

    public string AffixSuperior => "精良的";

    public string AffixBlighted => "凋落的";

    public string AffixBlightRavaged => "凋落蔓延";

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
        Prefix = "物品種類",
        DivinationCard = "命運卡",
        StackableCurrency = "可堆疊通貨",
        Jewel = "珠寶",
        DelveStackableSocketableCurrency = "掘獄可堆疊有插槽通貨",
        MetamorphSample = "鍊魔樣本",
        HeistTool = "劫盜工具",
        Amulet = "項鍊",
        Ring = "戒指",
        Belt = "腰帶",
        Gloves = "手套",
        Boots = "鞋子",
        BodyArmours = "胸甲",
        Helmets = "頭部",
        Shields = "盾",
        Quivers = "箭袋",
        LifeFlasks = "生命藥劑",
        ManaFlasks = "魔力藥劑",
        HybridFlasks = "複合藥劑",
        UtilityFlasks = "功能藥劑",
        ActiveSkillGems = "技能寶石",
        SupportSkillGems = "輔助寶石",
        Maps = "地圖",
        MapFragments = "地圖碎片",
        Contract = "契約書",
        Blueprint = "藍圖",
        MiscMapItems = "其它",
        Claws = "爪",
        Daggers = "匕首",
        Wands = "法杖",
        OneHandSwords = "單手劍",
        ThrustingOneHandSwords = "細劍",
        OneHandAxes = "單手斧",
        OneHandMaces = "單手錘",
        Bows = "弓",
        Staves = "長杖",
        TwoHandSwords = "雙手劍",
        TwoHandAxes = "雙手斧",
        TwoHandMaces = "雙手錘",
        Sceptres = "權杖",
        RuneDaggers = "符紋匕首",
        Warstaves = "征戰長杖",
        FishingRods = "魚竿",
        HeistGear = "劫盜裝備",
        HeistBrooch = "劫盜胸針",
        HeistTarget = "劫盜目標",
        HeistCloak = "劫盜披風",
        AbyssJewel = "深淵珠寶",
        Trinkets = "飾品",
        Logbooks = "探險日誌",
        MemoryLine = "記憶",
        SanctumResearch = "聖域研究",
        SanctumRelics = "聖物",
        Tinctures = "萃取物",
        Corpses = "屍體",
    };
}
