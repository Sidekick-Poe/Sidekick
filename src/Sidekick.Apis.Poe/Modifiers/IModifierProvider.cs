using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Modifiers
{
    public interface IModifierProvider : IInitializableService
    {
        /// <summary>
        /// Gets a modifier category from an api id. Allows for the internal use of an enum which is less error prone than hard coded string values.
        /// </summary>
        /// <param name="apiId">The api id to get a modifier category for.</param>
        /// <returns>The modifier category.</returns>
        ModifierCategory GetModifierCategory(string apiId);

        bool IsMatch(string id, string text);

        Dictionary<ModifierCategory, List<ModifierPattern>> Patterns { get; }

        Dictionary<string, List<ModifierPattern>> FuzzyDictionary { get; }
    }
}
