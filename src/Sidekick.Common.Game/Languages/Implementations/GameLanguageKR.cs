using System;

namespace Sidekick.Common.Game.Languages.Implementations
{
    [GameLanguage("Korean", "kr")]
    public class GameLanguageKR : IGameLanguage
    {
        public string LanguageCode => "kr";

        public Uri PoeTradeSearchBaseUrl => new("https://poe.game.daum.net/trade/search/");
        public Uri PoeTradeExchangeBaseUrl => new("https://poe.game.daum.net/trade/exchange/");
        public Uri PoeTradeApiBaseUrl => new("https://poe.game.daum.net/api/trade/");
        public Uri PoeCdnBaseUrl => new("https://web.poecdn.com/");

        public string RarityUnique => "고유";
        public string RarityRare => "희귀";
        public string RarityMagic => "마법";
        public string RarityNormal => "일반";
        public string RarityCurrency => "화폐";
        public string RarityGem => "젬";
        public string RarityDivinationCard => "점술 카드";

        public string DescriptionUnidentified => "미확인";
        public string DescriptionQuality => "퀄리티";
        public string DescriptionAlternateQuality => "대체 퀄리티";
        public string DescriptionIsRelic => "고유 유물";
        public string DescriptionCorrupted => "타락";
        public string DescriptionScourged => "스컬지";
        public string DescriptionSockets => "홈";
        public string DescriptionItemLevel => "아이템 레벨";
        public string DescriptionMapTier => "지도 등급";
        public string DescriptionItemQuantity => "아이템 수량";
        public string DescriptionItemRarity => "아이템 희귀도";
        public string DescriptionMonsterPackSize => "몬스터 무리 규모";
        public string DescriptionExperience => "경험치";
        public string DescriptionPhysicalDamage => "물리 피해";
        public string DescriptionElementalDamage => "원소 피해";
        public string DescriptionAttacksPerSecond => "초당 공격 횟수";
        public string DescriptionCriticalStrikeChance => "치명타 확률";
        public string DescriptionEnergyShield => "에너지 보호막";
        public string DescriptionArmour => "방어도";
        public string DescriptionEvasion => "회피";
        public string DescriptionChanceToBlock => "막기";
        public string DescriptionLevel => "레벨";
        public string DescriptionRequirements => "__TranslationRequired__:";

        public string PrefixSuperior => "상";
        public string PrefixBlighted => "역병";
        public string PrefixBlightRavaged => "역병에 유린당한";
        public string PrefixAnomalous => "기묘한";
        public string PrefixDivergent => "분기하는";
        public string PrefixPhantasmal => "환영의";

        public string InfluenceShaper => "쉐이퍼";
        public string InfluenceElder => "엘더";
        public string InfluenceCrusader => "성전사";
        public string InfluenceHunter => "사냥꾼";
        public string InfluenceRedeemer => "대속자";
        public string InfluenceWarlord => "전쟁군주";

        public ClassLanguage Classes => null;
    }
}
