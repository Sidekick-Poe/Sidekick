
namespace Sidekick.Apis.Poe.Languages.Implementations;

[GameLanguage("Korean", "ko")]
public class GameLanguageKo : IGameLanguage
{
    public string Code => "ko";

    public string PoeTradeBaseUrl => "https://poe.game.daum.net/trade/";
    public string PoeTradeApiBaseUrl => "https://poe.game.daum.net/api/trade/";
    public string Poe2TradeBaseUrl => "https://poe.game.daum.net/trade2/";
    public string Poe2TradeApiBaseUrl => "https://poe.game.daum.net/api/trade2/";
    public Uri PoeCdnBaseUrl => new("https://web.poecdn.com/");

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
    public string DescriptionMagicMonsters => "마법 몬스터";
    public string DescriptionRareMonsters => "희귀 몬스터";
    public string DescriptionRevivesAvailable => "부활 횟수";
    public string DescriptionWaystoneDropChance => "경로석 출현 확률";
    public string DescriptionAreaLevel => "지역 레벨";
    public string DescriptionUnusable => "아이템 착용 불가. 아이템 효과 미적용";
    public string DescriptionRequirements => "요구사항";
    public string DescriptionRequires => "요구 사항";
    public string DescriptionRequiresLevel => "레벨";
    public string DescriptionRequiresStr => "힘";
    public string DescriptionRequiresDex => "민첩";
    public string DescriptionRequiresInt => "지능";

    public string AffixSuperior => "상";
    public string AffixBlighted => "역병";
    public string AffixBlightRavaged => "역병에 유린당한";

    public string InfluenceShaper => "쉐이퍼 아이템";
    public string InfluenceElder => "엘더 아이템";
    public string InfluenceCrusader => "십자군 아이템";
    public string InfluenceHunter => "사냥꾼 아이템";
    public string InfluenceRedeemer => "대속자 아이템";
    public string InfluenceWarlord => "전쟁군주 아이템";

    public ClassLanguage Classes { get; } = new()
    {
        Prefix = "아이템 종류",
        DivinationCard = "점술 카드",
        StackableCurrency = "중첩 가능 화폐",
        Socketable = "홈에 장착 가능",
        Omen = "징조",
        Jewel = "주얼",
        DelveStackableSocketableCurrency = "탐광 중첩 및 결합형 화폐",
        HeistTool = "강탈 도구",
        Amulet = "목걸이",
        Ring = "반지",
        Belt = "허리띠",
        Gloves = "장갑",
        Boots = "장화",
        BodyArmours = "갑옷",
        Helmets = "투구",
        Shields = "방패",
        Quivers = "화살통",
        Focus = "집중구",
        LifeFlasks = "생명력 플라스크",
        ManaFlasks = "마나 플라스크",
        HybridFlasks = "하이브리드 플라스크",
        UtilityFlasks = "특수 플라스크",
        ActiveSkillGems = "스킬 젬",
        SupportSkillGems = "보조 젬",
        UncutSkillGems = "미가공 스킬 젬",
        UncutSupportGems = "미가공 보조 젬",
        UncutSpiritGems = "미가공 정신력 젬",
        Maps = "지도",
        MapFragments = "지도 조각",
        Contract = "계약",
        Blueprint = "도면",
        MiscMapItems = "기타 지도 아이템",
        Waystone = "경로석",
        Barya = "시련 주화",
        Ultimatum = "새겨진 결전",
        Tablet = "서판",
        Breachstone = "균열석",
        BossKey = "최종 열쇠",
        Claws = "클로",
        Daggers = "단검",
        Wands = "마법봉",
        OneHandSwords = "한손 검",
        ThrustingOneHandSwords = "날카로운 한손 검",
        OneHandAxes = "한손 도끼",
        OneHandMaces = "한손 철퇴",
        Bows = "활",
        Crossbows = "쇠뇌",
        Staves = "지팡이",
        TwoHandSwords = "양손 검",
        TwoHandAxes = "양손 도끼",
        TwoHandMaces = "양손 철퇴",
        Sceptres = "셉터",
        RuneDaggers = "룬 단검",
        Warstaves = "전쟁지팡이",
        Quarterstaves = "육척봉",
        Spears = "창",
        Bucklers = "버클러",
        FishingRods = "낚싯대",
        HeistGear = "강탈 장비",
        HeistBrooch = "강탈 브로치",
        HeistTarget = "강탈 대상",
        HeistCloak = "강탈 망토",
        AbyssJewel = "심연 주얼",
        Trinkets = "장신구",
        Logbooks = "탐험 일지",
        MemoryLine = "기억",
        SanctumRelics = "유물",
        Tinctures = "팅크",
        Corpses = "시신",
        SanctumResearch = "성역 연구",
        Grafts = "기생체",
        Wombgifts = "생명의 결실",
    };
}

