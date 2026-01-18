using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public abstract class DoublePropertyFilter : TradeFilter
{
    public override async Task<AutoSelectResult?> Initialize(Item item, ISettingsService settingsService)
    {
        var result = await base.Initialize(item, settingsService);
        if (result == null) return null;

        if (result.FillMinRange) NormalizeMinValue(result.NormalizeBy);
        if (result.FillMaxRange) NormalizeMaxValue(result.NormalizeBy);

        return result;
    }

    public string? ValuePrefix { get; init; }

    public string? ValueSuffix { get; init; }

    public required double Value { get; init; }

    public double OriginalValue { get; init; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public required bool NormalizeEnabled { get; set; }

    private void NormalizeMinValue(double normalizeBy)
    {
        if (!NormalizeEnabled || Value == 0)
        {
            Min = Value;
            return;
        }

        if (Value > 0)
        {
            var normalizedValue = (1 - normalizeBy) * Value;
            Min = Math.Round(normalizedValue, 2);
        }
        else
        {
            var normalizedValue = (1 + normalizeBy) * Value;
            Min = Math.Round(normalizedValue, 2);
        }
    }

    private void NormalizeMaxValue(double normalizeby)
    {
        if (!NormalizeEnabled || Value == 0)
        {
            Max = Value;
            return;
        }

        if (Value > 0)
        {
            var normalizedValue = (1 + normalizeby) * Value;
            Max = Math.Round(normalizedValue, 2);
        }
        else
        {
            var normalizedValue = (1 - normalizeby) * Value;
            Max = Math.Round(normalizedValue, 2);
        }
    }

}
