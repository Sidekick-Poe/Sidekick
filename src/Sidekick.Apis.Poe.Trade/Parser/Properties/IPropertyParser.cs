using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public interface IPropertyParser : IInitializableService
{
    ItemProperties Parse(ParsingItem parsingItem, ItemHeader header);

    void ParseAfterModifiers(Item item, ParsingItem parsingItem);

    Task<List<PropertyFilter>> GetFilters(Item item);

    void PrepareTradeRequest(Query query, Item item, List<PropertyFilter> propertyFilters);
}
