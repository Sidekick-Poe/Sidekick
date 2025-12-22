using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
namespace Sidekick.Apis.Poe.Extensions;

/// <summary>
///     Class containing extension methods for strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// A regular expression used to extract and process text within square brackets,
    /// optionally separated by pipes, for parsing modifier patterns within game data.
    /// </summary>
    /// <example>
    /// [ItemRarity|Rarity of Items] => Rarity of Items
    /// [Spell] => Spell
    /// </example>
    private static Regex SquareBracketPattern { get; } = new("\\[.*?\\|?([^\\|\\[\\]]*)\\]");

    public static string RemoveSquareBrackets(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        return SquareBracketPattern.Replace(text, "$1");
    }


    public static string? GetUrlSlugForLeague(this string? leagueId)
    {
        return leagueId?.Split('.').ElementAtOrDefault(1);
    }

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
