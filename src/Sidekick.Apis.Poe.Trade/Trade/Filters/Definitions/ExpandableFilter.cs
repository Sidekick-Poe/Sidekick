using Sidekick.Apis.Poe.Trade.Parser.Properties;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;

public class ExpandableFilter(ExpandablePropertiesDefinition definition, params TradeFilter[] filters) : TradeFilter(definition)
{
    public List<TradeFilter> Filters { get; } = filters.ToList();
}
