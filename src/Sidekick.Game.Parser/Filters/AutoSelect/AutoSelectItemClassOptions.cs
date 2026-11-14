using Sidekick.Common.Enums;
using Sidekick.Game.ItemClasses;
namespace Sidekick.Game.Parser.Filters.AutoSelect;

public record AutoSelectItemClassOption(ItemClass Type, string Label);

/// <summary>
///     Builds the item class options available to an item class auto select condition.
/// </summary>
public static class AutoSelectItemClassOptions
{
    /// <summary>
    ///     Gets one option per item class that exists in the game data, sorted by label.
    /// </summary>
    /// <param name="game">The game the definitions belong to.</param>
    /// <param name="definitions">The item class definitions loaded for the game.</param>
    /// <param name="selectedValues">The values already saved in the condition. These are always kept so they can be unchecked.</param>
    public static List<AutoSelectItemClassOption> Get(GameType game, IEnumerable<ItemClassDefinition> definitions, IEnumerable<string>? selectedValues = null)
    {
        var options = definitions
            .Where(x => x.Type != ItemClass.Unknown)
            .GroupBy(x => x.Type)
            .Select(x => new AutoSelectItemClassOption(x.Key, GetLabel(game, x.Key, x.ToList())))
            .ToList();

        foreach (var value in selectedValues ?? [])
        {
            if (!Enum.TryParse<ItemClass>(value, out var type) || !Enum.IsDefined(type) || type == ItemClass.Unknown) continue;
            if (options.Any(x => x.Type == type)) continue;

            options.Add(new AutoSelectItemClassOption(type, type.ToString()));
        }

        return options
            .OrderBy(x => x.Label, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    private static string GetLabel(GameType game, ItemClass type, List<ItemClassDefinition> definitions)
    {
        if (definitions.Count == 1) return definitions[0].Name ?? type.ToString();

        // Multiple game classes map to the same type (e.g. Maps and Map Keys). Use the primary game id, which is the first one declared on the enum.
        var primaryId = type.FindAttribute<ItemClassGameId>(x => x.Game == game)?.Id;
        return definitions.FirstOrDefault(x => x.Id == primaryId)?.Name ?? type.ToString();
    }
}
