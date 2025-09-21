using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade;

public interface ITradeFilterService
{
    /// <summary>
    /// Gets a list of modifier filters for a specific item.
    /// </summary>
    /// <param name="item">The item for which to get modifier filters.</param>
    /// <returns>The list of modifier filters.</returns>
    Task<List<ModifierFilter>> GetModifierFilters(Item item);

    /// <summary>
    /// Gets a list of pseudo modifier filters for a specific item.
    /// </summary>
    /// <param name="item">The item for which to get modifier filters.</param>
    /// <returns>The list of modifier filters.</returns>
    IEnumerable<PseudoFilter> GetPseudoModifierFilters(Item item);
}
