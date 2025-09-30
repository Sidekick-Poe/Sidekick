using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo.Filters;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade.Parser.Pseudo;

public interface IPseudoParser : IInitializableService
{
    void Parse(Item item);

    /// <summary>
    /// Gets a list of pseudo modifier filters for a specific item.
    /// </summary>
    /// <param name="item">The item for which to get modifier filters.</param>
    /// <returns>The list of modifier filters.</returns>
    List<PseudoFilter> GetFilters(Item item);
}
