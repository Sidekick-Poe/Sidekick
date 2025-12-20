using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Initialization;
namespace Sidekick.Apis.Poe.Trade.Parser.Stats;

public interface IStatParser : IInitializableService
{
    void Parse(Item item);

    Task<List<TradeFilter>> GetFilters(Item item);
}
