using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Parser.Modifiers;

public interface IModifierParser
{
    /// <summary>
    /// Parses the modifiers from the parsing item and returns a list of modifier lines.
    /// </summary>
    /// <param name="parsingItem">The item currently being parsed.</param>
    /// <param name="header">The item header.</param>
    /// <returns>The list of modifier lines. Each line may contain one or more modifiers which the user will then have to pick.</returns>
    List<ModifierLine> Parse(ParsingItem parsingItem, ItemHeader header);
}
