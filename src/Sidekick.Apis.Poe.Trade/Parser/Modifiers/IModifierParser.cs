using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Initialization;
namespace Sidekick.Apis.Poe.Trade.Parser.Modifiers;

public interface IModifierParser : IInitializableService
{
    /// <summary>
    /// Parses the modifiers from the parsing item and returns a list of modifier lines.
    /// </summary>
    /// <param name="item">The item currently being parsed.</param>
    void Parse(Item item);

    /// <summary>
    /// Gets a list of modifier filters for a specific item.
    /// </summary>
    /// <param name="item">The item for which to get modifier filters.</param>
    /// <returns>The list of modifier filters.</returns>
    Task<List<ModifierFilter>> GetFilters(Item item);
}
