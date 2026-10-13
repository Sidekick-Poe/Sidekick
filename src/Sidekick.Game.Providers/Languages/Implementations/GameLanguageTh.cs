
namespace Sidekick.Game.Providers.Languages.Implementations;

public class GameLanguageTh : IGameLanguage
{
    public string Code => "th";
    public string Label => "Thai";

    public string PoeTradeBaseUrl => "https://th.pathofexile.com/trade/";
    public string PoeTradeApiBaseUrl => "https://th.pathofexile.com/api/trade/";
    public string Poe2TradeBaseUrl => "https://th.pathofexile.com/trade2/";
    public string Poe2TradeApiBaseUrl => "https://th.pathofexile.com/api/trade2/";

    public string DescriptionPhysicalDamage => "ความเสียหายกายภาพ";
    public string DescriptionElementalDamage => "ความเสียหายธาตุ";
    public string DescriptionFireDamage => "ความเสียหายไฟ";
    public string DescriptionColdDamage => "ความเสียหายน้ำแข็ง";
    public string DescriptionLightningDamage => "ความเสียหายน้ำสายฟ้า";
    public string DescriptionChaosDamage => "ความเสียหายเคออส";
    public string DescriptionQualityCurrency => "ค่าคุณภาพ (เงินตรา)";
    public string DescriptionQualityScarabs => "ค่าคุณภาพ (สคารับ)";
    public string DescriptionQualityCards => "ค่าคุณภาพ (ไพ่พยากรณ์)";
    public string DescriptionQualityPackSize => "ค่าคุณภาพ (ขนาดกองมอนสเตอร์)";
    public string DescriptionQualityRarity => "ค่าคุณภาพ (ระดับความหายาก)";
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
}

