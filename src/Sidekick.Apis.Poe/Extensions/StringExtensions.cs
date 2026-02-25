using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Data.Items.Models;
namespace Sidekick.Apis.Poe.Extensions;

/// <summary>
///     Class containing extension methods for strings.
/// </summary>
public static class StringExtensions
{
    public static GameType GetGameFromLeagueId(this string? leagueId)
    {
        return leagueId
            ?.Split('.')
            .ElementAtOrDefault(0) switch
        {
            "poe2" => GameType.PathOfExile2,
            _ => GameType.PathOfExile1,
        };
    }

    private static readonly Regex parseCategoryPattern = new(@" \((?:implicit|enchant|crafted|veiled|fractured|scourge|crucible|rune|desecrated|mutated)\)");

    public static string RemoveCategory(this string text)
    {
        return parseCategoryPattern.Replace(text, string.Empty);
    }

    public static StatCategory ParseCategory(this string text)
    {
        var match = parseCategoryPattern.Match(text);
        if (!match.Success)
        {
            return StatCategory.Undefined;
        }

        var categoryText = match.Value.Trim(' ', '(', ')');
        return categoryText switch
        {
            "implicit" => StatCategory.Implicit,
            "enchant" => StatCategory.Enchant,
            "crafted" => StatCategory.Crafted,
            "veiled" => StatCategory.Veiled,
            "fractured" => StatCategory.Fractured,
            "scourge" => StatCategory.Scourge,
            "crucible" => StatCategory.Crucible,
            "rune" => StatCategory.Rune,
            "desecrated" => StatCategory.Desecrated,
            "mutated" => StatCategory.Mutated,
            _ => StatCategory.Undefined,
        };
    }
}
