using Sidekick.Common.Game.Items;

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

    public double? TotalDps { get; set; }

    public double? ElementalDps { get; set; }

    public double? PhysicalDps { get; set; }

    public DamageRange PhysicalDamage { get; init; } = new();

    public List<DamageRange> ElementalDamages { get; init; } = new();

    public DamageRange ChaosDamage { get; init; } = new();

    public double? ChaosDps { get; set; }

    public int? BaseDefencePercentile { get; init; }
}
