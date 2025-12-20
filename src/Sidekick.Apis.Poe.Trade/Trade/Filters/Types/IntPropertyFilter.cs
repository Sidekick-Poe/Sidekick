using Sidekick.Apis.Poe.Trade.Parser.Properties;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class IntPropertyFilter : TradeFilter
{
    internal IntPropertyFilter(PropertyDefinition definition)
        : base(definition)
    {
    }

    public string? ValuePrefix { get; init; }

    public string? ValueSuffix { get; init; }

    public required int Value { get; init; }

    public int OriginalValue { get; init; }

    public int? Min { get; set; }

    public int? Max { get; set; }

    public bool NormalizeEnabled { get; set; }
}
