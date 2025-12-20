using Sidekick.Apis.Poe.Trade.Parser.Properties;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class DoublePropertyFilter : TradeFilter
{
    internal DoublePropertyFilter(PropertyDefinition definition)
        : base(definition)
    {
    }

    public string? ValuePrefix { get; set; }

    public string? ValueSuffix { get; init; }

    public required double Value { get; init; }

    public double OriginalValue { get; init; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public bool NormalizeEnabled { get; set; }
}
