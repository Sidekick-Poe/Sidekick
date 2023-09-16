using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe
{
    public interface ITradeFilterService
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
        PropertyFilters GetPropertyFilters(Item item);
    }
}
