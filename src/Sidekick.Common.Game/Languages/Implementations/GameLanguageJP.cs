using System;

namespace Sidekick.Common.Game.Languages.Implementations
{
    [GameLanguage("Japanese", "jp")]
    public class GameLanguageJP : IGameLanguage
    {
        public string LanguageCode => "jp";

        public Uri PoeTradeSearchBaseUrl => new("https://jp.pathofexile.com/trade/search/");
        public Uri PoeTradeExchangeBaseUrl => new("https://jp.pathofexile.com/trade/exchange/");
        public Uri PoeTradeApiBaseUrl => new("https://jp.pathofexile.com/api/trade/");
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
        public string DescriptionIsRelic => "ユニークレリック";
        public string DescriptionCorrupted => "コラプト状態";
        public string DescriptionScourged => "スカージ";
        public string DescriptionSockets => "ソケット";
        public string DescriptionItemLevel => "アイテムレベル";
        public string DescriptionExperience => "経験値";
        public string DescriptionMapTier => "マップティア";
        public string DescriptionItemQuantity => "アイテム数量";
        public string DescriptionItemRarity => "アイテムレアリティ";
        public string DescriptionMonsterPackSize => "モンスターパックサイズ";
        public string DescriptionPhysicalDamage => "物理ダメージ";
        public string DescriptionElementalDamage => "元素ダメージ";
        public string DescriptionAttacksPerSecond => "秒間アタック回数";
        public string DescriptionCriticalStrikeChance => "クリティカル率";
        public string DescriptionEnergyShield => "エナジーシールド";
        public string DescriptionArmour => "アーマー";
        public string DescriptionEvasion => "回避力";
        public string DescriptionChanceToBlock => "ブロック率";
        public string DescriptionLevel => "レベル";
        public string DescriptionRequirements => "装備要求";

        public string PrefixSuperior => "上質な";
        public string PrefixBlighted => "ブライト";
        public string PrefixBlightRavaged => "ブライトに破壊された";
        public string PrefixAnomalous => "異常な";
        public string PrefixDivergent => "相違の";
        public string PrefixPhantasmal => "幻想の";

        public string InfluenceShaper => "シェイパー";
        public string InfluenceElder => "エルダー";
        public string InfluenceCrusader => "クルセイダー";
        public string InfluenceHunter => "ハンター";
        public string InfluenceRedeemer => "レディーマー";
        public string InfluenceWarlord => "ウォーロード";

        public ClassLanguage Classes { get; } = new ClassLanguage()
        {
            Prefix = "アイテムクラス: ",
            DivinationCard = "占いカード",
            StackableCurrency = "スタック可能カレンシー",
            Jewel = "ジュエル",
            DelveStackableSocketableCurrency = "デルヴスタック可能ソケット可能カレンシー",
            MetamorphSample = "メタモルフの臓器",
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
            UtilityFlasks = "ユーティリティフラスコ ユニーク",
            CriticalUtilityFlasks = "クリティカルユーティリティフラスコ ユニーク",
            ActiveSkillGems = "アクティブスキルジェム",
            SupportSkillGems = "サポートスキルジェム",
            Maps = "マップ",
            MapFragments = "マップの断片",
            Contract = "ハイスト依頼書",
            Blueprint = "ハイスト計画書",
            MiscMapItems = "その他マップアイテム",
            Claws = "鉤爪",
            Daggers = "短剣",
            Wands = "ワンド",
            OneHandSwords = "片手剣",
            ThrustingOneHandSwords = "刺突用片手剣",
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
            HeistTarget = "ハイスト目標",
            HeistCloak = "ハイストクローク",
            AbyssJewel = "アビスジュエル",
            Trinkets = "トリンケット",
            Logbooks = "エクスペディションログブック",
            Sentinel = "センチネル",
            MemoryLine = "メモリーライン"
        };
    }
}
