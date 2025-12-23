namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public abstract class DoublePropertyFilter : TradeFilter
{
    public string? ValuePrefix { get; init; }

    public string? ValueSuffix { get; init; }

    public required double Value { get; init; }

    public double OriginalValue { get; init; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public bool NormalizeEnabled { get; init; }
}
