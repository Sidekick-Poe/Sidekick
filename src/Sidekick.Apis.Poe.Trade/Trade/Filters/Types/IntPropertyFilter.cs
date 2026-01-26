using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class IntPropertyFilter : TradeFilter, INormalizableFilter
{
    public override async Task<AutoSelectResult?> Initialize(Item item, ISettingsService settingsService)
    {
        var result = await base.Initialize(item, settingsService);
        if (result == null) return null;

        if (result.FillMinRange) Min = (int)NormalizeMinValue(result.NormalizeBy);
        if (result.FillMaxRange) Max = (int)NormalizeMaxValue(result.NormalizeBy);

        return result;
    }

    public string? ValuePrefix { get; init; }

    public string? ValueSuffix { get; init; }

    public required int Value { get; init; }

    public double NormalizeValue => Value;

    public int OriginalValue { get; init; }

    public int? Min { get; set; }

    public int? Max { get; set; }

    public required bool NormalizeEnabled { get; set; }

    public double NormalizeMinValue(double normalizeBy)
    {
        if (!NormalizeEnabled || Value == 0)
        {
            return Value;
        }

        if (Value > 0)
        {
            var normalizedValue = (1 - normalizeBy) * Value;
            return Math.Round(normalizedValue, 0);
        }
        else
        {
            var normalizedValue = (1 + normalizeBy) * Value;
            return Math.Round(normalizedValue, 0);
        }
    }

    public double NormalizeMaxValue(double normalizeby)
    {
        if (!NormalizeEnabled || Value == 0)
        {
            return Value;
        }

        if (Value > 0)
        {
            var normalizedValue = (1 + normalizeby) * Value;
            return Math.Round(normalizedValue, 0);
        }
        else
        {
            var normalizedValue = (1 - normalizeby) * Value;
            return Math.Round(normalizedValue, 0);
        }
    }

}
