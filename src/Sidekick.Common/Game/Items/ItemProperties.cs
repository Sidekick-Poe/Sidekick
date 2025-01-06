namespace Sidekick.Common.Game.Items;

public class ItemProperties
{
    public bool Unidentified { get; set; }

    public int ItemLevel { get; set; }

    public bool Corrupted { get; set; }

    public int Armour { get; set; }

    public int EnergyShield { get; set; }

    public int EvasionRating { get; set; }

    public int BlockChance { get; set; }

    public int Quality { get; set; }

    public int GemLevel { get; set; }

    public int MapTier { get; set; }

    public int AreaLevel { get; set; }

    public int ItemQuantity { get; set; }

    public int ItemRarity { get; set; }

    public int MonsterPackSize { get; set; }

    public bool Blighted { get; set; }

    public bool BlightRavaged { get; set; }

    public Influences Influences { get; init; } = new();

    public double CriticalHitChance { get; set; }

    public double AttacksPerSecond { get; set; }

    public DamageRange? PhysicalDamage { get; set; }

    public DamageRange? FireDamage { get; set; }

    public DamageRange? ColdDamage { get; set; }

    public DamageRange? LightningDamage { get; set; }

    public DamageRange? ChaosDamage { get; set; }

    private readonly double? physicalDpsOverride;

    public double? PhysicalDps
    {
        get
        {
            if (physicalDpsOverride != null) return physicalDpsOverride;

            var total = 0d;
            if (PhysicalDamage != null) total += PhysicalDamage.GetDps(AttacksPerSecond);

            return total;
        }
        init => physicalDpsOverride = value;
    }

    private readonly double? elementalDpsOverride;

    public double? ElementalDps
    {
        get
        {
            if (elementalDpsOverride != null) return elementalDpsOverride;

            var total = 0d;
            if (FireDamage != null) total += FireDamage.GetDps(AttacksPerSecond);
            if (ColdDamage != null) total += ColdDamage.GetDps(AttacksPerSecond);
            if (LightningDamage != null) total += LightningDamage.GetDps(AttacksPerSecond);

            return total;
        }
        init => elementalDpsOverride = value;
    }

    private readonly double? chaosDpsOverride;

    public double? ChaosDps
    {
        get
        {
            if (chaosDpsOverride != null) return chaosDpsOverride;

            var total = 0d;
            if (ChaosDamage != null) total += ChaosDamage.GetDps(AttacksPerSecond);

            return total;
        }
        init => chaosDpsOverride = value;
    }

    private readonly double? totalDpsOverride;

    public double? TotalDps
    {
        get
        {
            if (totalDpsOverride != null) return totalDpsOverride;

            var total = 0d;
            if (PhysicalDamage != null) total += PhysicalDamage.GetDps(AttacksPerSecond);
            if (FireDamage != null) total += FireDamage.GetDps(AttacksPerSecond);
            if (ColdDamage != null) total += ColdDamage.GetDps(AttacksPerSecond);
            if (LightningDamage != null) total += LightningDamage.GetDps(AttacksPerSecond);
            if (ChaosDamage != null) total += ChaosDamage.GetDps(AttacksPerSecond);

            return total;
        }
        init => totalDpsOverride = value;
    }

    public double? TotalDamage
    {
        get
        {
            var total = 0d;
            if (PhysicalDamage != null) total += (PhysicalDamage.Min + PhysicalDamage.Max) / 2;
            if (FireDamage != null) total += (FireDamage.Min + FireDamage.Max) / 2;
            if (ColdDamage != null) total += (ColdDamage.Min + ColdDamage.Max) / 2;
            if (LightningDamage != null) total += (LightningDamage.Min + LightningDamage.Max) / 2;
            if (ChaosDamage != null) total += (ChaosDamage.Min + ChaosDamage.Max) / 2;

            return total;
        }
    }

    public int? BaseDefencePercentile { get; init; }
}
