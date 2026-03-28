namespace Sidekick.Apis.Poe.Items;

public class DamageRange(int min, int max)
{
    public int Min { get; init; } = min;

    public int Max { get; init; } = max;

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
        return Math.Round(((Min + Max) / 2d) * attacksPerSecond, 1);
    }
}
