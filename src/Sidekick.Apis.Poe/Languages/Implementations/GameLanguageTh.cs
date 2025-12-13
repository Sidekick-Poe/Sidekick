
namespace Sidekick.Apis.Poe.Languages.Implementations;

[GameLanguage("Thai (Unstable)", "th")]
public class GameLanguageTh : IGameLanguage
{
    public string Code => "th";

    public string PoeTradeBaseUrl => "https://th.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://th.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://th.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://th.pathofexile.com/api/trade2/";
    public Uri PoeCdnBaseUrl => new("https://web.poecdn.com/");

    public string RarityUnique => "ยูนิค";
    public string RarityRare => "แรร์";
    public string RarityMagic => "เมจิก";
    public string RarityNormal => "ปกติ";
    public string RarityCurrency => "เงินตรา";
    public string RarityGem => "หิน";
    public string RarityDivinationCard => "ไพ่พยากรณ์";

    public string DescriptionRarity => "ความหายาก";
    public string DescriptionUnidentified => "ยังไม่ได้ตรวจสอบ";
    public string DescriptionQuality => "ค่าคุณภาพ";
    public string DescriptionLevel => "เลเวล";
    public string DescriptionCorrupted => "มีมลทิน";
    public string DescriptionSockets => "รู";
    public string DescriptionItemLevel => "เลเวลไอเทม";
    public string DescriptionExperience => "ค่าประสบการณ์";
    public string DescriptionPhysicalDamage => "ความเสียหายกายภาพ";
    public string DescriptionElementalDamage => "ความเสียหายธาตุ";
    public string DescriptionFireDamage => "ความเสียหายไฟ";
    public string DescriptionColdDamage => "ความเสียหายน้ำแข็ง";
    public string DescriptionLightningDamage => "ความเสียหายน้ำสายฟ้า";
    public string DescriptionChaosDamage => "ความเสียหายเคออส";
    public string DescriptionEnergyShield => "โล่พลังงาน";
    public string DescriptionEnergyShieldAlternate => "";
    public string DescriptionArmour => "ค่าเกราะ";
    public string DescriptionEvasion => "อัตราการหลบหลีก";
    public string DescriptionChanceToBlock => "โอกาสบล็อค";
    public string DescriptionBlockChance => "โอกาสบล็อค";
    public string DescriptionSpirit => "พลังวิญญาณ";
    public string DescriptionAttacksPerSecond => "จำนวนครั้งการโจมตีต่อวินาที";
    public string DescriptionCriticalStrikeChance => "โอกาสคริติคอล";
    public string DescriptionCriticalHitChance => "โอกาสปะทะคริติคอล";
    public string DescriptionMapTier => "ระดับแผนที่";
    public string DescriptionReward => "ของรางวัล";
    public string DescriptionItemQuantity => "จำนวนของไอเทม";
    public string DescriptionItemRarity => "ระดับความหายากของไอเทม";
    public string DescriptionMonsterPackSize => "ขนาดกองมอนสเตอร์";
    public string DescriptionMagicMonsters => "มอนสเตอร์เมจิก";
    public string DescriptionRareMonsters => "มอนสเตอร์แรร์";
    public string DescriptionRevivesAvailable => "จำนวนสิทธิ์คืนชีพ";
    public string DescriptionWaystoneDropChance => "โอกาสดรอปศิลานำทาง";
    public string DescriptionAreaLevel => "ด่านเลเวล";
    public string DescriptionUnusable => "คุณไม่สามารถใช้ไอเทมชิ้นนี้ได้ Stats ของไอเทมนี้จะไม่มีผล";
    public string DescriptionRequirements => "เงื่อนไข";
    public string DescriptionRequires => "ต้องการ";
    public string DescriptionRequiresLevel => "เลเวล";
    public string DescriptionRequiresStr => "Str";
    public string DescriptionRequiresDex => "Dex";
    public string DescriptionRequiresInt => "Int";

    public string AffixSuperior => "Superior";
    public string AffixBlighted => "Blighted";
    public string AffixBlightRavaged => "Blight-ravaged";

    public string InfluenceShaper => "ไอเทมเชปเปอร์";
    public string InfluenceElder => "ไอเทมเอลเดอร์";
    public string InfluenceCrusader => "ไอเทมผู้พิชิตอธรรม";
    public string InfluenceHunter => "ไอเทมผู้พิชิตเหยื่อ";
    public string InfluenceRedeemer => "ไอเทมผู้พิชิตบาป";
    public string InfluenceWarlord => "ไอเทมผู้พิชิตศึก";

    public string RegexIncreased => "";
    public string RegexReduced => "";
    public string RegexMore => "";
    public string RegexLess => "";
    public string RegexFaster => "";
    public string RegexSlower => "";

    public ClassLanguage Classes { get; } = new()
    {
        Prefix = "ชนิดไอเทม",
        DivinationCard = "ไพ่พยากรณ์",
        StackableCurrency = "เงินตรารวมกองได้",
        Socketable = "ไอเทมใส่รู",
        Omen = "ลางบอกเหตุ",
        Jewel = "จิวเวล",
        DelveStackableSocketableCurrency = "เงินตรามีรูของเหมืองแบบรวมกองได้",
        HeistTool = "เครื่องมือกองโจร",
        Amulet = "สร้อย",
        Ring = "แหวน",
        Belt = "เข็มขัด",
        Gloves = "ถุงมือ",
        Boots = "รองเท้า",
        BodyArmours = "เสื้อเกราะ",
        Helmets = "หมวก",
        Shields = "โล่",
        Quivers = "ซองธนู",
        Focus = "โฟกัส",
        LifeFlasks = "ขวดยาพลังชีวิต",
        ManaFlasks = "ขวดยามานา",
        HybridFlasks = "ขวดยาผสม",
        UtilityFlasks = "ขวดยาช่วยเหลือ",
        ActiveSkillGems = "หินสกิล",
        SupportSkillGems = "หินเสริม",
        UncutSkillGems = "หินสกิลหยาบ",
        UncutSupportGems = "หินเสริมหยาบ",
        UncutSpiritGems = "หินพลังวิญญาณหยาบ",
        Maps = "แผนที่",
        MapFragments = "ชิ้นส่วนแผนที่",
        Contract = "สัญญาจ้าง",
        Blueprint = "พิมพ์เขียว",
        MiscMapItems = "ไอเทมแผนที่อื่นๆ",
        Waystone = "ศิลานำทาง",
        Barya = "เหรียญบททดสอบ",
        Ultimatum = "คำขาดจารึก",
        Tablet = "แผ่นหิน",
        Breachstone = "ศิลาบรีช",
        BossKey = "กุญแจอภิมหาบอส",
        Claws = "กรงเล็บ",
        Daggers = "มีด",
        Wands = "ไม้กายสิทธิ์",
        OneHandSwords = "ดาบมือเดียว",
        ThrustingOneHandSwords = "ดาบแทง",
        OneHandAxes = "ขวานมือเดียว",
        OneHandMaces = "กระบองมือเดียว",
        Bows = "ธนู",
        Crossbows = "หน้าไม้",
        Staves = "ไม้พลอง",
        TwoHandSwords = "ดาบสองมือ",
        TwoHandAxes = "ขวานสองมือ",
        TwoHandMaces = "กระบองสองมือ",
        Sceptres = "คทา",
        RuneDaggers = "มีดอาคม",
        Warstaves = "ไม้พลองสงคราม",
        Quarterstaves = "ไม้พลองวรยุทธ์",
        Spears = "หอก",
        Bucklers = "บัคเลอร์",
        FishingRods = "เบ็ดตกปลา",
        HeistGear = "อุปกรณ์สวมใส่กองโจร",
        HeistBrooch = "เข็มกลัดกองโจร",
        HeistTarget = "เป้าหมายโจรกรรม",
        HeistCloak = "ผ้าคลุมกองโจร",
        AbyssJewel = "จิวเวลอะบิส",
        Trinkets = "เครื่องประดับโจร",
        Logbooks = "สมุดปูมเดินทางกองสำรวจ",
        MemoryLine = "ความทรงจำ",
        SanctumRelics = "วัตถุตกทอด",
        Tinctures = "ยาเคลือบอาวุธ",
        Corpses = "ศพ",
        SanctumResearch = "บทวิจัยเทวสถาน",
        Grafts = "หัตถ์",
        Wombgifts = "ผลฝากครรภ์",
    };
}

