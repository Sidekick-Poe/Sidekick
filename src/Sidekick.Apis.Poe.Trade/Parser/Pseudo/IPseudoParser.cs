using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Common.Initialization;
using Sidekick.Game.Items;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo;

public interface IPseudoParser : IInitializableService
{
    void Parse(Item item);

    Task<List<TradeFilter>> GetFilters(Item item);
}
