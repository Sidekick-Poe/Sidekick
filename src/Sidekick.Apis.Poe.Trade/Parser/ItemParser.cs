using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Trade.Parser.Definition;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo;
using Sidekick.Apis.Poe.Trade.Parser.Stats;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;
using Sidekick.Data;
using Sidekick.Data.Extensions;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser;

public class ItemParser
(
    ILogger<ItemParser> logger,
    IStatParser statParser,
    IPseudoParser pseudoParser,
    IPropertyParser propertyParser,
    ICurrentGameLanguage currentGameLanguage,
    IItemDefinitionParser itemDefinitionParser,
    ISettingsService settingsService
) : IItemParser
{
    private Regex? UnusablePattern { get; set; }

    public int Priority => 100;

    private GameType Game { get; set; }

    public async Task Initialize()
    {
        var unusableRegex = Regex.Escape(currentGameLanguage.Language.DescriptionUnusable);
        unusableRegex += @"[\n\r]+" + RawText.SeparatorPattern + @"[\n\r]+";
        UnusablePattern = new Regex(unusableRegex, RegexOptions.Compiled);
        Game = await settingsService.GetGame();
    }

    public Item ParseItem(string? text)
    {
        if (string.IsNullOrEmpty(text)) throw new UnparsableException(text);

        try
        {
            text = NormalizeText(text);
            text = RemoveUnusableLine(text);

            var item = new Item(Game, currentGameLanguage.Language, text);

            // Rarity property is required for definition parsing. This means that it must be parsed first.
            propertyParser.GetDefinition<RarityProperty>().Parse(item);

            itemDefinitionParser.Parse(item);
            propertyParser.Parse(item);
            statParser.Parse(item);
            propertyParser.ParseAfterStats(item);
            pseudoParser.Parse(item);

            return item;
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Could not parse item.");
            throw new UnparsableException(text);
        }
    }

    private string RemoveUnusableLine(string itemText)
    {
        return UnusablePattern?.Replace(itemText, string.Empty) ?? itemText;
    }

    /// <summary>
    /// True if the parenthetical content is numeric (digits, hyphen, comma, space)
    /// </summary>
    static bool IsNumericParenContent(string content) =>
        !string.IsNullOrEmpty(content) && Regex.IsMatch(content, @"^[\d\-\s,]+$");

    /// <summary>
    /// Remove only numeric parenthetical groups like (39-42) but keep text ones like (implicit)
    /// </summary>
    static string RemoveNumericParentheses(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return Regex.Replace(input, @"\(([^)]*)\)", m =>
        {
            var inner = m.Groups[1].Value;
            return IsNumericParenContent(inner) ? "" : m.Value;
        });
    }

    /// <summary>
    /// Replace digit sequences with '#' and digit+% sequences with '#%'
    /// </summary>
    static string ReplaceDigitsWithHash(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        // First replace digit sequences immediately followed by % with #%
        var step1 = Regex.Replace(input, @"\d+%", "#%");

        // Then replace remaining digit sequences with #
        var step2 = Regex.Replace(step1, @"\d+", "#");

        // Collapse multiple spaces and trim
        return Regex.Replace(step2, @"\s+", " ").Trim();
    }

    /// <summary>
    /// Extract leading numeric value (handles + or - and decimals)
    /// </summary>
    static bool TryExtractLeadingValue(string line, out decimal value)
    {
        value = 0m;
        if (string.IsNullOrWhiteSpace(line)) return false;
        var m = Regex.Match(line.TrimStart(), @"^([+-]?\d+(\.\d+)?)");
        if (!m.Success) return false;
        return decimal.TryParse(m.Groups[1].Value, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }

    /// <summary>
    /// Aggregates numeric-prefixed lines by normalized key (digits -> # or #%).
    /// Only collapses groups with more than one matching line; singletons remain unchanged.
    /// </summary>
    private static string NormalizeText(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        var lines = input.Split(["\r\n", "\n"], StringSplitOptions.None).ToList();

        // Pattern to detect lines that start with a numeric prefix (e.g., "42(...)" or "+31 " or "31% ")
        var leadingNumberPattern = new Regex(@"^\s*[+-]?\d+(\([^\)]*\))?%?\s+.*$", RegexOptions.Compiled);

        // Map normalized key -> list of (index, originalLine, numericValue)
        var groups = new Dictionary<string, List<(int idx, string line, decimal value)>>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Only consider lines that start with a numeric prefix
            if (!leadingNumberPattern.IsMatch(line)) continue;
            if (!TryExtractLeadingValue(line, out var val)) continue;

            // Remove the leading numeric and optional numeric parentheses and optional percent sign
            var remainder = Regex.Replace(line.TrimStart(), @"^[+-]?\d+(\([^\)]*\))?%?\s*", "");

            // Remove numeric parentheses only (keep textual parentheses)
            var remainderNoNumericParens = RemoveNumericParentheses(remainder);

            // Build normalized key by replacing digits with # or #%
            var normalized = ReplaceDigitsWithHash(remainderNoNumericParens);

            if (!groups.TryGetValue(normalized, out var list))
            {
                list = new List<(int, string, decimal)>();
                groups[normalized] = list;
            }

            list.Add((i, line, val));
        }

        // Determine which normalized keys are duplicates (count > 1)
        var keysToAggregate = groups.Where(kv => kv.Value.Count > 1).ToList();

        // If nothing to aggregate, return original unchanged
        if (keysToAggregate.Count == 0) return input;

        // Build a set of indices to remove (only lines that are part of aggregated groups)
        var indicesToRemove = new HashSet<int>();
        var aggregatedLines = new List<string>();

        foreach (var kv in keysToAggregate)
        {
            var entries = kv.Value;

            // Sum numeric values
            var sum = entries.Sum(e => e.value);

            // Representative label: take the first occurrence's remainder with numeric parentheses removed
            var firstRemainder = Regex.Replace(entries[0].line.TrimStart(), @"^[+-]?\d+(\([^\)]*\))?%?\s*", "");
            var repLabel = RemoveNumericParentheses(firstRemainder).Trim();
            repLabel = Regex.Replace(repLabel, @"\s+", " ").Trim();

            // Format sum (integer if whole, otherwise up to 2 decimals)
            var formatted = sum % 1 == 0 ? ((int)sum).ToString(CultureInfo.InvariantCulture)
                                            : sum.ToString("0.##", CultureInfo.InvariantCulture);

            aggregatedLines.Add($"{formatted} {repLabel}");

            // Mark indices for removal
            foreach (var e in entries) indicesToRemove.Add(e.idx);
        }

        // Build cleaned lines: remove aggregated original lines and any modifier header lines { ... }
        var cleaned = new List<string>();
        for (var i = 0; i < lines.Count; i++)
        {
            if (indicesToRemove.Contains(i)) continue;
            var l = lines[i];
            if (l.TrimStart().StartsWith("{") || l.Trim().StartsWith("}")) continue;
            cleaned.Add(l);
        }

        // Find insertion point: after the "Item Level:" separator if present
        var itemLevelIndex = cleaned.FindIndex(l => l.TrimStart().StartsWith("Item Level:", StringComparison.OrdinalIgnoreCase));
        var insertIndex = cleaned.Count;
        if (itemLevelIndex >= 0)
        {
            var sepIndex = cleaned.FindIndex(itemLevelIndex, l => l.Trim() == "--------");
            insertIndex = sepIndex >= 0 ? sepIndex + 1 : itemLevelIndex + 1;
        }

        // Insert a separator and aggregated lines (preserve the order of aggregatedLines as discovered)
        var block = new List<string> { "--------" };
        block.AddRange(aggregatedLines);

        cleaned.InsertRange(insertIndex, block);

        // Collapse duplicate separators
        for (var i = cleaned.Count - 2; i >= 0; i--)
        {
            if (cleaned[i].Trim() == "--------" && cleaned[i + 1].Trim() == "--------")
                cleaned.RemoveAt(i + 1);
        }

        return string.Join("\n", cleaned);
    }
}
