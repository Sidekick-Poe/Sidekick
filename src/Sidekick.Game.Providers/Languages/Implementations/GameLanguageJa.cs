
namespace Sidekick.Game.Languages.Implementations;

public class GameLanguageJa : IGameLanguage
{
    public string Code => "ja";
    public string Label => "Japanese";

    public string PoeTradeBaseUrl => "https://jp.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://jp.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://jp.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://jp.pathofexile.com/api/trade2/";

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
    public string DescriptionSplit => "スプリット";
    public string DescriptionMirrored => "ミラー状態";
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
    public string DescriptionMoreMaps => "マップ量が上昇";
    public string DescriptionMoreScarabs => "スカラベ量が上昇";
    public string DescriptionMoreCurrency => "カレンシー量が上昇";
    public string DescriptionMoreCards => "占いカード増加";
    public string DescriptionQualityCurrency => "品質 (カレンシー)";
    public string DescriptionQualityScarabs => "品質 (スカラベ)";
    public string DescriptionQualityCards => "品質 (占いカード)";
    public string DescriptionQualityPackSize => "品質 (パックサイズ)";
    public string DescriptionQualityRarity => "品質 (レアリティ)";
    public string DescriptionMagicMonsters => "マジックモンスター";
    public string DescriptionRareMonsters => "レアモンスター";
    public string DescriptionRevivesAvailable => "復活が利用可能";
    public string DescriptionWaystoneDropChance => "ウェイストーンドロップ確率";
    public string DescriptionAreaLevel => "エリアレベル";
    public string DescriptionMemoryStrands => "メモリーストランド";
    public string DescriptionUnusable => "このアイテムを使用できません。アイテムの効果は無視されます";
    public string DescriptionRequirements => "装備要求";
    public string DescriptionRequires => "装備条件";
    public string DescriptionRequiresLevel => "レベル";
    public string DescriptionRequiresStr => "筋力";
    public string DescriptionRequiresDex => "器用さ";
    public string DescriptionRequiresInt => "知性";
    public string DescriptionHeistWings => "情報を聞いた区画";
    public string DescriptionHeistRoutes => "情報を聞いた脱出ルート";
    public string DescriptionHeistRooms => "情報を聞いた報酬部屋";
    public string DescriptionHeistLockpicking => "必要ジョブ 錠前破り (レベル #)";
    public string DescriptionHeistDemolition => "必要ジョブ 爆破 (レベル #)";
    public string DescriptionHeistAgility => "必要ジョブ 敏捷性 (レベル #)";
    public string DescriptionHeistCounterThaumaturgy => "必要ジョブ 対魔術 (レベル #)";
    public string DescriptionHeistTrap => "必要ジョブ 罠解除 (レベル #)";
    public string DescriptionHeistPerception => "必要ジョブ 知覚能力 (レベル #)";
    public string DescriptionHeistBruteForce => "必要ジョブ 怪力 (レベル #)";
    public string DescriptionHeistDeception => "必要ジョブ 欺瞞 (レベル #)";
    public string DescriptionHeistEngineering => "必要ジョブ 工作 (レベル #)";
    public string DescriptionHeistModerateValue => "中程度な価値";
    public string DescriptionHeistHighValue => "高価値";
    public string DescriptionHeistPrecious => "貴重";
    public string DescriptionHeistPriceless => "プライスレス";

    public string AffixSuperior => "上質な";
    public string AffixBlighted => "ブライト";
    public string AffixBlightRavaged => "ブライトに破壊された";
}

