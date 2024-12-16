namespace Sidekick.Common.Game.Languages.Implementations;

[GameLanguage("Thai", "th")]
public class GameLanguageTH : IGameLanguage
{
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

    public string DescriptionUnidentified => "ยังไม่ได้ตรวจสอบ";
    public string DescriptionQuality => "ค่าคุณภาพ";
    public string DescriptionAlternateQuality => "ค่าคุณภาพแบบพิเศษ";
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
    public string DescriptionArmour => "ค่าเกราะ";
    public string DescriptionEvasion => "อัตราการหลบหลีก";
    public string DescriptionChanceToBlock => "โอกาสบล็อค";
    public string DescriptionAttacksPerSecond => "จำนวนครั้งการโจมตีต่อวินาที";
    public string DescriptionCriticalStrikeChance => "โอกาสคริติคอล";
    public string DescriptionMapTier => "ระดับแผนที่";
    public string DescriptionItemQuantity => "จำนวนของไอเทม";
    public string DescriptionItemRarity => "ระดับความหายากของไอเทม";
    public string DescriptionMonsterPackSize => "ขนาดกองมอนสเตอร์";
    public string DescriptionRequirements => "เงื่อนไข";
    public string DescriptionAreaLevel => "ด่านเลเวล";

    public string AffixSuperior => "Superior";
    public string AffixBlighted => "Blighted";
    public string AffixBlightRavaged => "Blight-ravaged";
    public string AffixAnomalous => "Anomalous";
    public string AffixDivergent => "Divergent";
    public string AffixPhantasmal => "Phantasmal";

    public string InfluenceShaper => "ไอเทมเชปเปอร์";
    public string InfluenceElder => "ไอเทมเอลเดอร์";
    public string InfluenceCrusader => "ไอเทมผู้พิชิตอธรรม";
    public string InfluenceHunter => "ไอเทมผู้พิชิตเหยื่อ";
    public string InfluenceRedeemer => "ไอเทมผู้พิชิตบาป";
    public string InfluenceWarlord => "ไอเทมผู้พิชิตศึก";

    public ClassLanguage Classes { get; } = new()
    {
        Prefix = "ชนิดไอเทม",
        DivinationCard = "ไพ่พยากรณ์",
        StackableCurrency = "เงินตรารวมกองได้",
        Jewel = "จิวเวล",
        DelveStackableSocketableCurrency = "เงินตรามีรูของเหมืองแบบรวมกองได้",
        MetamorphSample = "ชิ้นส่วนตัวอย่างเมตามอร์ฟ",
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
        LifeFlasks = "ขวดยาพลังชีวิต",
        ManaFlasks = "ขวดยามานา",
        HybridFlasks = "ขวดยาผสม",
        UtilityFlasks = "ขวดยาช่วยเหลือ",
        ActiveSkillGems = "หินสกิล",
        SupportSkillGems = "หินเสริม",
        Maps = "แผนที่",
        MapFragments = "ชิ้นส่วนแผนที่",
        Contract = "สัญญาจ้าง",
        Blueprint = "พิมพ์เขียว",
        MiscMapItems = "ไอเทมแผนที่อื่นๆ",
        Claws = "กรงเล็บ",
        Daggers = "มีด",
        Wands = "ไม้กายสิทธิ์",
        OneHandSwords = "ดาบมือเดียว",
        ThrustingOneHandSwords = "ดาบแทง",
        OneHandAxes = "ขวานมือเดียว",
        OneHandMaces = "กระบองมือเดียว",
        Bows = "ธนู",
        Staves = "ไม้พลอง",
        TwoHandSwords = "ดาบสองมือ",
        TwoHandAxes = "ขวานสองมือ",
        TwoHandMaces = "กระบองสองมือ",
        Sceptres = "คทา",
        RuneDaggers = "มีดอาคม",
        Warstaves = "ไม้พลองสงคราม",
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
    };
}

