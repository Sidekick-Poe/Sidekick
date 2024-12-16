namespace Sidekick.Common.Game.Items;

public class DamageRange(double min, double max)
{
    public double Min { get; init; } = min;

    public double Max { get; init; } = max;

    public override string ToString()
    {
        return $"{Min:F0}-{Max:F0}";
    }

    public string ToDisplayString()
    {
        return $"{Min:F0}-{Max:F0}";
    }

    public double GetDps(double attacksPerSecond)
    {
        return Math.Round(((Min + Max) / 2) * attacksPerSecond, 1);
    }
}
