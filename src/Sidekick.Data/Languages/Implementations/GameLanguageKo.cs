
namespace Sidekick.Data.Languages.Implementations;

public class GameLanguageKo : IGameLanguage
{
    public string Code => "ko";
    public string Label => "Korean";

    public string PoeTradeBaseUrl => "https://poe.kakaogames.com/trade/";
    public string PoeTradeApiBaseUrl => "https://poe.kakaogames.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://poe.kakaogames.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://poe.kakaogames.com/api/trade2/";

    public string RarityUnique => "고유";
    public string RarityRare => "희귀";
    public string RarityMagic => "마법";
    public string RarityNormal => "일반";
    public string RarityCurrency => "화폐";
    public string RarityGem => "젬";
    public string RarityDivinationCard => "점술 카드";

    public string DescriptionRarity => "아이템 희귀도";
    public string DescriptionUnidentified => "미확인";
    public string DescriptionQuality => "퀄리티";
    public string DescriptionLevel => "레벨";
    public string DescriptionCorrupted => "타락";
    public string DescriptionSplit => "분할됨";
    public string DescriptionMirrored => "복제";
    public string DescriptionSockets => "홈";
    public string DescriptionItemLevel => "아이템 레벨";
    public string DescriptionExperience => "경험치";
    public string DescriptionPhysicalDamage => "물리 피해";
    public string DescriptionElementalDamage => "원소 피해";
    public string DescriptionFireDamage => "화염 피해";
    public string DescriptionColdDamage => "냉기 피해";
    public string DescriptionLightningDamage => "번개 피해";
    public string DescriptionChaosDamage => "카오스 피해";
    public string DescriptionEnergyShield => "에너지 보호막";
    public string DescriptionEnergyShieldAlternate => "";
    public string DescriptionArmour => "방어도";
    public string DescriptionEvasion => "회피";
    public string DescriptionChanceToBlock => "막기 확률";
    public string DescriptionBlockChance => "막기 확률";
    public string DescriptionSpirit => "정신력";
    public string DescriptionAttacksPerSecond => "초당 공격 횟수";
    public string DescriptionCriticalStrikeChance => "치명타 확률";
    public string DescriptionCriticalHitChance => "치명타 명중 확률";
    public string DescriptionMapTier => "지도 등급";
    public string DescriptionReward => "보상";
    public string DescriptionItemQuantity => "아이템 수량";
    public string DescriptionItemRarity => "아이템 희귀도";
    public string DescriptionMonsterPackSize => "몬스터 무리 규모";
    public string DescriptionMoreMaps => "지도 더 많음";
    public string DescriptionMoreScarabs => "갑충석 더 많음";
    public string DescriptionMoreCurrency => "화폐 더 많음";
    public string DescriptionMoreCards => "점술 카드 증폭";
    public string DescriptionQualityCurrency => "퀄리티 (화폐)";
    public string DescriptionQualityScarabs => "퀄리티 (갑충석)";
    public string DescriptionQualityCards => "퀄리티 (점술 카드)";
    public string DescriptionQualityPackSize => "퀄리티 (무리 규모)";
    public string DescriptionQualityRarity => "퀄리티 (희귀도)";
    public string DescriptionMagicMonsters => "마법 몬스터";
    public string DescriptionRareMonsters => "희귀 몬스터";
    public string DescriptionRevivesAvailable => "부활 횟수";
    public string DescriptionWaystoneDropChance => "경로석 출현 확률";
    public string DescriptionAreaLevel => "지역 레벨";
    public string DescriptionMemoryStrands => "기억 가닥";
    public string DescriptionUnusable => "아이템 착용 불가. 아이템 효과 미적용";
    public string DescriptionRequirements => "요구사항";
    public string DescriptionRequires => "요구 사항";
    public string DescriptionRequiresLevel => "레벨";
    public string DescriptionRequiresStr => "힘";
    public string DescriptionRequiresDex => "민첩";
    public string DescriptionRequiresInt => "지능";
    public string DescriptionHeistWings => "부속 건물 드러남";
    public string DescriptionHeistRoutes => "탈출 경로 드러남";
    public string DescriptionHeistRooms => "보물 창고 드러남";
    public string DescriptionHeistLockpicking => "자물쇠 따기 필요(#레벨)";
    public string DescriptionHeistDemolition => "파괴 공작 필요(#레벨)";
    public string DescriptionHeistAgility => "민첩함 필요(#레벨)";
    public string DescriptionHeistCounterThaumaturgy => "반-마석학 필요(#레벨)";
    public string DescriptionHeistTrap => "덫 해제 필요(#레벨)";
    public string DescriptionHeistPerception => "통찰력 필요(#레벨)";
    public string DescriptionHeistBruteForce => "완력 필요(#레벨)";
    public string DescriptionHeistDeception => "기만 필요(#레벨)";
    public string DescriptionHeistEngineering => "공학 필요(#레벨)";
    public string DescriptionHeistModerateValue => "보통 가치";
    public string DescriptionHeistHighValue => "높은 가치";
    public string DescriptionHeistPrecious => "귀중한 가치";
    public string DescriptionHeistPriceless => "환산 불가능한 가치";

    public string AffixSuperior => "상";
    public string AffixBlighted => "역병";
    public string AffixBlightRavaged => "역병에 유린당한";
}

