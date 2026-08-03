
namespace Sidekick.Data.Languages.Implementations;

public class GameLanguageTh : IGameLanguage
{
    public string Code => "th";
    public string Label => "Thai";

    public string PoeTradeBaseUrl => "https://th.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://th.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://th.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://th.pathofexile.com/api/trade2/";

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
    public string DescriptionSplit => "ผ่านการแยก";
    public string DescriptionMirrored => "ถูกสะท้อน";
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
    public string DescriptionMoreMaps => "เพิ่มแผนที่ อีก";
    public string DescriptionMoreScarabs => "เพิ่มสคารับ อีก";
    public string DescriptionMoreCurrency => "เพิ่มเงินตรา อีก";
    public string DescriptionMoreCards => "เพิ่มไพ่พยากรณ์";
    public string DescriptionQualityCurrency => "ค่าคุณภาพ (เงินตรา)";
    public string DescriptionQualityScarabs => "ค่าคุณภาพ (สคารับ)";
    public string DescriptionQualityCards => "ค่าคุณภาพ (ไพ่พยากรณ์)";
    public string DescriptionQualityPackSize => "ค่าคุณภาพ (ขนาดกองมอนสเตอร์)";
    public string DescriptionQualityRarity => "ค่าคุณภาพ (ระดับความหายาก)";
    public string DescriptionMagicMonsters => "มอนสเตอร์เมจิก";
    public string DescriptionRareMonsters => "มอนสเตอร์แรร์";
    public string DescriptionRevivesAvailable => "จำนวนสิทธิ์คืนชีพ";
    public string DescriptionWaystoneDropChance => "โอกาสดรอปศิลานำทาง";
    public string DescriptionAreaLevel => "ด่านเลเวล";
    public string DescriptionMemoryStrands => "เส้นความทรงจำ";
    public string DescriptionUnusable => "คุณไม่สามารถใช้ไอเทมชิ้นนี้ได้ Stats ของไอเทมนี้จะไม่มีผล";
    public string DescriptionRequirements => "เงื่อนไข";
    public string DescriptionRequires => "ต้องการ";
    public string DescriptionRequiresLevel => "เลเวล";
    public string DescriptionRequiresStr => "Str";
    public string DescriptionRequiresDex => "Dex";
    public string DescriptionRequiresInt => "Int";
    public string DescriptionHeistWings => "ปีกที่เปิดเผย";
    public string DescriptionHeistRoutes => "เส้นทางหลบหนีที่เปิดเผย";
    public string DescriptionHeistRooms => "ห้องของรางวัลที่เปิดเผย";
    public string DescriptionHeistLockpicking => "ต้องมี สะเดาะกุญแจ (ระดับ #)";
    public string DescriptionHeistDemolition => "ต้องมี รื้อถอน (ระดับ #)";
    public string DescriptionHeistAgility => "ต้องมี ความคล่องตัว (ระดับ #)";
    public string DescriptionHeistCounterThaumaturgy => "ต้องมี โต้มนต์มณี (ระดับ #)";
    public string DescriptionHeistTrap => "ต้องมี ปลดกับดัก (ระดับ #)";
    public string DescriptionHeistPerception => "ต้องมี การรับรู้ (ระดับ #)";
    public string DescriptionHeistBruteForce => "ต้องมี เอากำลังเข้าแลก (ระดับ #)";
    public string DescriptionHeistDeception => "ต้องมี กลฉ้อฉล (ระดับ #)";
    public string DescriptionHeistEngineering => "ต้องมี งานช่าง (ระดับ #)";
    public string DescriptionHeistModerateValue => "มีมูลค่าพอประมาณ";
    public string DescriptionHeistHighValue => "มีมูลค่าสูง";
    public string DescriptionHeistPrecious => "มีมูลค่าสูงส่ง";
    public string DescriptionHeistPriceless => "มีมูลค่าอันมิอาจเทียบ";

    public string AffixSuperior => "Superior";
    public string AffixBlighted => "Blighted";
    public string AffixBlightRavaged => "Blight-ravaged";
}

