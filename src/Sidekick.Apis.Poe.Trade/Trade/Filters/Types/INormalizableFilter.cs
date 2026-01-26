namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public interface INormalizableFilter
{
    bool Checked { get; }
    double NormalizeValue { get; }
    bool NormalizeEnabled { get; }

    double NormalizeMinValue(double normalizeBy)
    {
        if (!NormalizeEnabled || NormalizeValue == 0)
        {
            return NormalizeValue;
        }

        if (NormalizeValue > 0)
        {
            var normalizedValue = (1 - normalizeBy) * NormalizeValue;
            return Math.Round(normalizedValue, 2);
        }
        else
        {
            var normalizedValue = (1 + normalizeBy) * NormalizeValue;
            return Math.Round(normalizedValue, 2);
        }
    }

    double NormalizeMaxValue(double normalizeby)
    {
        if (!NormalizeEnabled || NormalizeValue == 0)
        {
            return NormalizeValue;
        }

        if (NormalizeValue > 0)
        {
            var normalizedValue = (1 + normalizeby) * NormalizeValue;
            return Math.Round(normalizedValue, 2);
        }
        else
        {
            var normalizedValue = (1 - normalizeby) * NormalizeValue;
            return Math.Round(normalizedValue, 2);
        }
    }

}
