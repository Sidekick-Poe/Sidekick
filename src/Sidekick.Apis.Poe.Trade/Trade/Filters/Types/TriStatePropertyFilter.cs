using Sidekick.Apis.Poe.Trade.Parser.Properties;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class TriStatePropertyFilter : TradeFilter
{
    internal TriStatePropertyFilter(
        PropertyDefinition definition) : base(definition)
    {
    }

    public new bool? Checked { get; set; }
}
