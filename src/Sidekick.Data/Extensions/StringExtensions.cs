using System.Text.RegularExpressions;
namespace Sidekick.Data.Extensions;

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
}
