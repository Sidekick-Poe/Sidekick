using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;
using Sidekick.Common.Initialization;
namespace Sidekick.Apis.Poe.Trade.Parser.Stats;

public interface IStatParser : IInitializableService
{
    void Parse(Item item);

    Task<List<StatFilter>> GetFilters(Item item);
}
