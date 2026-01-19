using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public abstract class DoublePropertyFilter : TradeFilter, INormalizableFilter
{
    public override async Task<AutoSelectResult?> Initialize(Item item, ISettingsService settingsService)
    {
        var result = await base.Initialize(item, settingsService);
        if (result == null) return null;

        if (result.FillMinRange) Min = ((INormalizableFilter)this).NormalizeMinValue(result.NormalizeBy);
        if (result.FillMaxRange) Max = ((INormalizableFilter)this).NormalizeMaxValue(result.NormalizeBy);

        return result;
    }

    public string? ValuePrefix { get; init; }

    public string? ValueSuffix { get; init; }

    public required double Value { get; init; }

    public double NormalizeValue => Value;

    public double OriginalValue { get; init; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public required bool NormalizeEnabled { get; set; }

}
