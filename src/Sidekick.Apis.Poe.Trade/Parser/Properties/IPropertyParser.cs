using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public interface IPropertyParser : IInitializableService
{
    TDefinition GetDefinition<TDefinition>() where TDefinition : PropertyDefinition;

    void Parse(Item item);

    void ParseAfterStats(Item item);

    Task<List<TradeFilter>> GetFilters(Item item);
}
