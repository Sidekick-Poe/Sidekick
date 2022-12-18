using System;

namespace Sidekick.Common.Game.Languages.Implementations
{
    [GameLanguage("Traditional Chinese", "zh")]
    public class GameLanguageZHTW : IGameLanguage
    {
        public string LanguageCode => "zh";

        public Uri PoeTradeSearchBaseUrl => new("http://web.poe.garena.tw/trade/search/");
        public Uri PoeTradeExchangeBaseUrl => new("http://web.poe.garena.tw/trade/exchange/");
        public Uri PoeTradeApiBaseUrl => new("http://web.poe.garena.tw/api/trade/");
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
        public string DescriptionIsRelic => "古典傳奇";
        public string DescriptionCorrupted => "已汙染";
        public string DescriptionScourged => "災魘";
        public string DescriptionSockets => "插槽";
        public string DescriptionItemLevel => "物品等級";
        public string DescriptionExperience => "經驗值";
        public string DescriptionPhysicalDamage => "物理傷害";
        public string DescriptionElementalDamage => "元素傷害";
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

        public string PrefixSuperior => "精良的";
        public string PrefixBlighted => "凋落的";
        public string PrefixAnomalous => "異常的";
        public string PrefixDivergent => "相異的";
        public string PrefixPhantasmal => "幻影的";

        public string InfluenceShaper => "塑者之物";
        public string InfluenceElder => "尊師之物";
        public string InfluenceCrusader => "聖戰軍王物品";
        public string InfluenceHunter => "狩獵者物品";
        public string InfluenceRedeemer => "救贖者物品";
        public string InfluenceWarlord => "總督軍物品";

        public ClassLanguage Classes => null;
    }
}
