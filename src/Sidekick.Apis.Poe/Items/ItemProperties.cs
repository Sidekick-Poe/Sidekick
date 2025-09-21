namespace Sidekick.Apis.Poe.Items;

public class ItemProperties
{
    public bool Unidentified { get; set; }

    public int ItemLevel { get; set; }

    public bool Corrupted { get; set; }

    public int Armour { get; set; }

    public int ArmourWithQuality => CalculateValueWithQuality(Armour);

    public int EnergyShield { get; set; }

    public int EnergyShieldWithQuality => CalculateValueWithQuality(EnergyShield);

    public int EvasionRating { get; set; }

    public int EvasionRatingWithQuality => CalculateValueWithQuality(EvasionRating);

    public int Spirit { get; set; }

    public int BlockChance { get; set; }

    public int Quality { get; set; }

    public int GemLevel { get; set; }

    public int MapTier { get; set; }

    public string? Reward { get; set; }

    public int AreaLevel { get; set; }

    public int ItemQuantity { get; set; }

    public int ItemRarity { get; set; }

    public int MonsterPackSize { get; set; }

    public bool Blighted { get; set; }

    public bool BlightRavaged { get; set; }

    public Influences Influences { get; } = new();

    public List<Socket>? Sockets { get; set; }

    public double CriticalHitChance { get; set; }

    public double AttacksPerSecond { get; set; }

    public DamageRange? PhysicalDamage { get; set; }

    public DamageRange? PhysicalDamageWithQuality
    {
        get
        {
            if (PhysicalDamage == null) return null;

            var min = CalculateValueWithQuality(PhysicalDamage.Min);
            var max = CalculateValueWithQuality(PhysicalDamage.Max);
            return new DamageRange(min, max);
        }
    }

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

    public double? PhysicalDpsWithQuality
    {
        get
        {
            if (physicalDpsOverride != null) return physicalDpsOverride;

            var total = 0d;
            if (PhysicalDamageWithQuality != null) total += PhysicalDamageWithQuality.GetDps(AttacksPerSecond);

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

    public double? TotalDpsWithQuality
    {
        get
        {
            if (totalDpsOverride != null) return totalDpsOverride;

            var total = 0d;
            if (PhysicalDamageWithQuality != null) total += PhysicalDamageWithQuality.GetDps(AttacksPerSecond);
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
            if (PhysicalDamage != null) total += (PhysicalDamage.Min + PhysicalDamage.Max) / 2d;
            if (FireDamage != null) total += (FireDamage.Min + FireDamage.Max) / 2d;
            if (ColdDamage != null) total += (ColdDamage.Min + ColdDamage.Max) / 2d;
            if (LightningDamage != null) total += (LightningDamage.Min + LightningDamage.Max) / 2d;
            if (ChaosDamage != null) total += (ChaosDamage.Min + ChaosDamage.Max) / 2d;

            return total;
        }
    }

    public double? TotalDamageWithQuality
    {
        get
        {
            var total = 0d;
            if (PhysicalDamageWithQuality != null) total += (PhysicalDamageWithQuality.Min + PhysicalDamageWithQuality.Max) / 2d;
            if (FireDamage != null) total += (FireDamage.Min + FireDamage.Max) / 2d;
            if (ColdDamage != null) total += (ColdDamage.Min + ColdDamage.Max) / 2d;
            if (LightningDamage != null) total += (LightningDamage.Min + LightningDamage.Max) / 2d;
            if (ChaosDamage != null) total += (ChaosDamage.Min + ChaosDamage.Max) / 2d;

            return total;
        }
    }

    public List<string> AugmentedProperties { get; } = [];

    public string? ClusterJewelGrantText { get; set; }

    public int? ClusterJewelPassiveCount { get; set; }

    public int CalculateValueWithQuality(int value)
    {
        if (Quality >= 20)
        {
            return value;
        }

        // Step 1: Calculate the base value (value at 0% quality)
        var baseValue = Math.Round(value / (1 + Quality / 100d), MidpointRounding.ToPositiveInfinity);

        // Step 2: Adjust the base value for the target quality
        return (int)(baseValue * 1.2);
    }

    public int GetMaximumNumberOfLinks()
    {
        if (Sockets == null) return 0;
        return Sockets.Count != 0 ? Sockets.GroupBy(x => x.Group).Max(x => x.Count()) : 0;
    }
}
