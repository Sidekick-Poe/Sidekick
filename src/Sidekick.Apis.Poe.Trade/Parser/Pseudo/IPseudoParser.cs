using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo;

public interface IPseudoParser : IInitializableService
{
    void Parse(Item item);

    List<PseudoFilter> GetFilters(Item item);
}
