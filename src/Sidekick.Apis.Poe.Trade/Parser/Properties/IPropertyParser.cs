using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public interface IPropertyParser : IInitializableService
{
    ItemProperties Parse(ParsingItem parsingItem, ItemHeader header);

    Task<List<BooleanPropertyFilter>> GetFilters(Item item);

    void PrepareTradeRequest(SearchFilters searchFilters, Item item, PropertyFilters propertyFilters);
}
