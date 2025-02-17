using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Apis.Poe.Trade.Requests.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Parser.Properties;

public interface IPropertyParser : IInitializableService
{
    ItemProperties Parse(ParsingItem parsingItem);

    void ParseAfterModifiers(ParsingItem parsingItem, ItemProperties properties, List<ModifierLine> modifierLines);

    List<BooleanPropertyFilter> GetFilters(Item item);

    void PrepareTradeRequest(SearchFilters searchFilters, Item item, PropertyFilters propertyFilters);
}
