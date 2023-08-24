using System.Collections.Generic;
using Sidekick.Apis.Poe.Parser;
using Sidekick.Common.Game.Items.Modifiers;
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

        /// <summary>
        /// Parses the modifiers from the parsing item and returns a list of modifier lines.
        /// </summary>
        /// <param name="parsingItem">The item currently being parsed.</param>
        /// <returns>The list of modifier lines. Each line may contain one or more modifiers which the user will then have to pick.</returns>
        List<ModifierLine> Parse(ParsingItem parsingItem);

        bool IsMatch(string id, string text);
    }
}
