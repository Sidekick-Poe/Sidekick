using System.Text.Json.Serialization;

namespace Sidekick.Common.Game.Items;

public class DamageRange
{
    public double Min { get; set; }
    public double Max { get; set; }
    public DamageType Type { get; set; }

    public override string ToString()
    {
        return $"{Min:F0}-{Max:F0}";
    }

    public string ToDisplayString()
    {
        return $"{Min:F0}-{Max:F0}";
    }

    public bool HasValue()
    {
        return Min > 0 || Max > 0;
    }
} 