
namespace Sidekick.Apis.Poe.Languages.Implementations;

[GameLanguage("Japanese", "ja")]
public class GameLanguageJa : IGameLanguage
{
    public string Code => "ja";

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

    public string DescriptionRarity => "レアリティ";
    public string DescriptionUnidentified => "未鑑定";
    public string DescriptionQuality => "品質";
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
    public string DescriptionEnergyShieldAlternate => "";
    public string DescriptionArmour => "アーマー";
    public string DescriptionEvasion => "回避力";
    public string DescriptionChanceToBlock => "ブロック率";
    public string DescriptionBlockChance => "ブロック率";
    public string DescriptionSpirit => "スピリット";
    public string DescriptionAttacksPerSecond => "秒間アタック回数";
    public string DescriptionCriticalStrikeChance => "クリティカル率";
    public string DescriptionCriticalHitChance => "クリティカルヒット率";
    public string DescriptionMapTier => "マップティア";
    public string DescriptionReward => "報酬";
    public string DescriptionItemQuantity => "アイテム数量";
    public string DescriptionItemRarity => "アイテムレアリティ";
    public string DescriptionMonsterPackSize => "モンスターパックサイズ";
    public string DescriptionAreaLevel => "エリアレベル";
    public string DescriptionUnusable => "このアイテムを使用できません。アイテムの効果は無視されます";
    public string DescriptionRequirements => "装備要求";
    public string DescriptionRequires => "装備条件";
    public string DescriptionRequiresLevel => "レベル";
    public string DescriptionRequiresStr => "筋力";
    public string DescriptionRequiresDex => "器用さ";
    public string DescriptionRequiresInt => "知性";

    public string AffixSuperior => "上質な";
    public string AffixBlighted => "ブライト";
    public string AffixBlightRavaged => "ブライトに破壊された";

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
        Socketable = "ソケット可能",
        Omen = "お告げ",
        Jewel = "ジュエル",
        DelveStackableSocketableCurrency = "デルヴスタック可能ソケット可能カレンシー",
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
        Focus = "フォーカス",
        LifeFlasks = "ライフフラスコ",
        ManaFlasks = "マナフラスコ",
        HybridFlasks = "ハイブリッドフラスコ",
        UtilityFlasks = "ユーティリティフラスコ",
        ActiveSkillGems = "スキルジェム",
        SupportSkillGems = "サポートジェム",
        UncutSkillGems = "スキルジェムの原石",
        UncutSupportGems = "サポートジェムの原石",
        UncutSpiritGems = "スピリットジェムの原石",
        Maps = "マップ",
        MapFragments = "マップの断片",
        Contract = "依頼書",
        Blueprint = "計画書",
        MiscMapItems = "その他マップアイテム",
        Waystone = "ウェイストーン",
        Barya = "試練のコイン",
        Ultimatum = "刻印されたアルティメイタム",
        Tablet = "石板",
        Breachstone = "ブリーチストーン",
        BossKey = "ピナクルキー",
        Claws = "鉤爪",
        Daggers = "短剣",
        Wands = "ワンド",
        OneHandSwords = "片手剣",
        ThrustingOneHandSwords = "刺突剣",
        OneHandAxes = "片手斧",
        OneHandMaces = "片手メイス",
        Bows = "弓",
        Crossbows = "クロスボウ",
        Staves = "スタッフ",
        TwoHandSwords = "両手剣",
        TwoHandAxes = "両手斧",
        TwoHandMaces = "両手メイス",
        Sceptres = "セプター",
        RuneDaggers = "ルーンの短剣",
        Warstaves = "ウォースタッフ",
        Quarterstaves = "クォータースタッフ",
        Spears = "槍",
        Bucklers = "バックラー",
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

