namespace Sidekick.Common.Game.Items;

public class Properties
{
    public bool Identified { get; init; }

    public int ItemLevel { get; init; }

    public bool Corrupted { get; init; }

    public int Armor { get; init; }

    public int EnergyShield { get; init; }

    public int Evasion { get; init; }

    public int ChanceToBlock { get; init; }

    public int Quality { get; init; }

    public bool AlternateQuality { get; init; }

    public int GemLevel { get; init; }

    public bool Anomalous { get; init; }

    public bool Divergent { get; init; }

    public bool Phantasmal { get; init; }

    public int MapTier { get; init; }

    public int AreaLevel { get; init; }

    public int ItemQuantity { get; init; }

    public int ItemRarity { get; init; }

    public int MonsterPackSize { get; init; }

    public bool Blighted { get; init; }

    public bool BlightRavaged { get; init; }

    public double CriticalStrikeChance { get; init; }

    public double AttacksPerSecond { get; init; }

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
