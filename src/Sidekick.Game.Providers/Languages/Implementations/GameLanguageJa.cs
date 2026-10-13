
namespace Sidekick.Game.Providers.Languages.Implementations;

public class GameLanguageJa : IGameLanguage
{
    public string Code => "ja";
    public string Label => "Japanese";

    public string PoeTradeBaseUrl => "https://jp.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://jp.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://jp.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://jp.pathofexile.com/api/trade2/";

    public string DescriptionPhysicalDamage => "物理ダメージ";
    public string DescriptionElementalDamage => "元素ダメージ";
    public string DescriptionFireDamage => "火ダメージ";
    public string DescriptionColdDamage => "冷気ダメージ";
    public string DescriptionLightningDamage => "雷ダメージ";
    public string DescriptionChaosDamage => "混沌ダメージ";
    public string DescriptionQualityCurrency => "品質 (カレンシー)";
    public string DescriptionQualityScarabs => "品質 (スカラベ)";
    public string DescriptionQualityCards => "品質 (占いカード)";
    public string DescriptionQualityPackSize => "品質 (パックサイズ)";
    public string DescriptionQualityRarity => "品質 (レアリティ)";
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
}

