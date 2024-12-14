using System.Globalization;

namespace Sidekick.Common.Game.Items;

public static class DamageRangeExtensions
{
    public static string ToDisplayString(this DamageRange? range)
    {
        if (range == null || (range.Min <= 0 && range.Max <= 0))
        {
            return string.Empty;
        }
        return $"{range.Min:F0}-{range.Max:F0}";
    }

    public static bool HasValue(this DamageRange? range)
    {
        return range != null && (range.Min > 0 || range.Max > 0);
    }

    public static decimal ToDecimal(this double value)
    {
        return Convert.ToDecimal(value);
    }

    public static double CalculateElementalDps(this IEnumerable<DamageRange> ranges, double attacksPerSecond)
    {
        if (ranges == null || !ranges.Any())
        {
            return 0;
        }

        var elementalDamage = ranges.Sum(x => (x.Min + x.Max) / 2.0);
        return elementalDamage * attacksPerSecond;
    }
} 