using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Trade;

public interface ITradeFilterService : IInitializableService
{
    /// <summary>
    /// Gets a list of modifier filters for a specific item.
    /// </summary>
    /// <param name="item">The item for which to get modifier filters.</param>
    /// <returns>The list of modifier filters.</returns>
    IEnumerable<ModifierFilter> GetModifierFilters(Item item);

    /// <summary>
    /// Gets a list of pseudo modifier filters for a specific item.
    /// </summary>
    /// <param name="item">The item for which to get modifier filters.</param>
    /// <returns>The list of modifier filters.</returns>
    IEnumerable<PseudoModifierFilter> GetPseudoModifierFilters(Item item);

    /// <summary>
    /// Gets a list of property filters for a specific item.
    /// </summary>
    /// <param name="item">The item for which to get property filters.</param>
    /// <returns>The property filters.</returns>
    Task<PropertyFilters> GetPropertyFilters(Item item);
}
