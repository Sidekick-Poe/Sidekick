using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public interface IPropertyParser : IInitializableService
{
    void Parse(Item item);

    void ParseAfterModifiers(Item item);

    Task<List<PropertyFilter>> GetFilters(Item item);

    void PrepareTradeRequest(Query query, Item item, List<PropertyFilter> propertyFilters);
}
