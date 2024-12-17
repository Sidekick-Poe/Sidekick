namespace Sidekick.Common.Game.Languages.Implementations;

[GameLanguage("Japanese", "jp")]
public class GameLanguageJP : IGameLanguage
{
    public string PoeTradeBaseUrl => "https://jp.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://jp.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://jp.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://jp.pathofexile.com/api/trade2/";
    public Uri PoeCdnBaseUrl => new("https://web.poecdn.com/");

    public string RarityUnique => "ユニーク";
    public string RarityRare => "レア";
    public string RarityMagic => "マジック";
    public string RarityNormal => "ノーマル";
    public string RarityCurrency => "カレンシー";
    public string RarityGem => "ジェム";
    public string RarityDivinationCard => "占いカード";

    public string DescriptionUnidentified => "未鑑定";
    public string DescriptionQuality => "品質";
    public string DescriptionAlternateQuality => "代替品質";
    public string DescriptionLevel => "レベル";
    public string DescriptionCorrupted => "コラプト状態";
    public string DescriptionSockets => "ソケット";
    public string DescriptionItemLevel => "アイテムレベル";
    public string DescriptionExperience => "経験値";
    public string DescriptionPhysicalDamage => "物理ダメージ";
    public string DescriptionElementalDamage => "元素ダメージ";
    public string DescriptionFireDamage => "火ダメージ";
    public string DescriptionColdDamage => "冷気ダメージ";
    public string DescriptionLightningDamage => "雷ダメージ";
    public string DescriptionChaosDamage => "混沌ダメージ";
    public string DescriptionEnergyShield => "エナジーシールド";
    public string DescriptionArmour => "アーマー";
    public string DescriptionEvasion => "回避力";
    public string DescriptionChanceToBlock => "ブロック率";
    public string DescriptionAttacksPerSecond => "秒間アタック回数";
    public string DescriptionCriticalStrikeChance => "クリティカル率";
    public string DescriptionMapTier => "マップティア";
    public string DescriptionItemQuantity => "アイテム数量";
    public string DescriptionItemRarity => "アイテムレアリティ";
    public string DescriptionMonsterPackSize => "モンスターパックサイズ";
    public string DescriptionRequirements => "装備要求";
    public string DescriptionAreaLevel => "エリアレベル";

    public string AffixSuperior => "上質な";
    public string AffixBlighted => "ブライト";
    public string AffixBlightRavaged => "ブライトに破壊された";
    public string AffixAnomalous => "異常な";
    public string AffixDivergent => "相違の";
    public string AffixPhantasmal => "幻想の";

    public string InfluenceShaper => "シェイパーアイテム";
    public string InfluenceElder => "エルダーアイテム";
    public string InfluenceCrusader => "クルセイダーアイテム";
    public string InfluenceHunter => "ハンターアイテム";
    public string InfluenceRedeemer => "レディーマーアイテム";
    public string InfluenceWarlord => "ウォーロードアイテム";

    public ClassLanguage Classes { get; } = new()
    {
        Prefix = "アイテムクラス",
        DivinationCard = "占いカード",
        StackableCurrency = "スタック可能カレンシー",
        Jewel = "ジュエル",
        DelveStackableSocketableCurrency = "デルヴスタック可能ソケット可能カレンシー",
        MetamorphSample = "メタモルフサンプル",
        HeistTool = "ハイストツール",
        Amulet = "アミュレット",
        Ring = "指輪",
        Belt = "ベルト",
        Gloves = "手袋",
        Boots = "靴",
        BodyArmours = "鎧",
        Helmets = "兜",
        Shields = "盾",
        Quivers = "矢筒",
        LifeFlasks = "ライフフラスコ",
        ManaFlasks = "マナフラスコ",
        HybridFlasks = "ハイブリッドフラスコ",
        UtilityFlasks = "ユーティリティフラスコ",
        ActiveSkillGems = "スキルジェム",
        SupportSkillGems = "サポートジェム",
        Maps = "マップ",
        MapFragments = "マップの断片",
        Contract = "依頼書",
        Blueprint = "計画書",
        MiscMapItems = "その他マップアイテム",
        Claws = "鉤爪",
        Daggers = "短剣",
        Wands = "ワンド",
        OneHandSwords = "片手剣",
        ThrustingOneHandSwords = "刺突剣",
        OneHandAxes = "片手斧",
        OneHandMaces = "片手メイス",
        Bows = "弓",
        Staves = "スタッフ",
        TwoHandSwords = "両手剣",
        TwoHandAxes = "両手斧",
        TwoHandMaces = "両手メイス",
        Sceptres = "セプター",
        RuneDaggers = "ルーンの短剣",
        Warstaves = "ウォースタッフ",
        FishingRods = "釣り竿",
        HeistGear = "ハイストギア",
        HeistBrooch = "ハイストブローチ",
        HeistTarget = "ハイストターゲット",
        HeistCloak = "ハイストクローク",
        AbyssJewel = "アビスジュエル",
        Trinkets = "トリンケット",
        Logbooks = "エクスペディションログブック",
        MemoryLine = "記憶",
        SanctumRelics = "レリック",
        Tinctures = "チンキ",
        Corpses = "死体",
        SanctumResearch = "サンクタム調査書",
    };
}

