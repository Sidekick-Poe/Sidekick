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

    public double? TotalDps
    {
        get
        {
            var total = 0d;
            if (PhysicalDamage != null) total += PhysicalDamage.GetDps(AttacksPerSecond);
            if (FireDamage != null) total += FireDamage.GetDps(AttacksPerSecond);
            if (ColdDamage != null) total += ColdDamage.GetDps(AttacksPerSecond);
            if (LightningDamage != null) total += LightningDamage.GetDps(AttacksPerSecond);
            if (ChaosDamage != null) total += ChaosDamage.GetDps(AttacksPerSecond);

            return total;
        }
    }

    public int? BaseDefencePercentile { get; init; }
}
