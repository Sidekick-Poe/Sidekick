using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public interface IPropertyParser : IInitializableService
{
    ItemProperties Parse(ParsingItem parsingItem, ItemHeader header);

    void ParseAfterModifiers(ParsingItem parsingItem, ItemHeader header, ItemProperties properties, List<ModifierLine> modifierLines);

    Task<List<PropertyFilter>> GetFilters(Item item);

    void PrepareTradeRequest(Query query, Item item, List<PropertyFilter> propertyFilters);
}
