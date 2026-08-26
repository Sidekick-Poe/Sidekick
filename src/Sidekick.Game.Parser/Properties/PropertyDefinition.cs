using System.Globalization;
using System.Text.RegularExpressions;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
namespace Sidekick.Game.Parser.Properties;

public abstract class PropertyDefinition
{
    public abstract string Label { get; }

    public virtual void Parse(Item item) {}

    public virtual void ParseAfterStats(Item item) {}

    public virtual Task<TradeFilter?> GetFilter(Item item) { return Task.FromResult<TradeFilter?>(null); }

    protected static bool GetBool(Regex pattern, OriginalText originalText) => originalText.TryParseRegex(pattern, out _);

    protected static bool GetBool(Regex pattern, OriginalBlock originalBlock) => originalBlock.TryParseRegex(pattern, out _);

    protected static string? GetString(Regex pattern, OriginalText originalText)
    {
        return originalText.TryParseRegex(pattern, out var match) ? match.Groups[1].Value.Trim(' ', ':') : null;
    }

    protected static int GetInt(Regex pattern, OriginalText originalText)
    {
        if (!originalText.TryParseRegex(pattern, out var match)) return 0;

        return int.TryParse(match.Groups[1].Value, out var result) ? result : 0;
    }

    protected static int GetInt(Regex pattern, OriginalBlock originalBlock)
    {
        if (!originalBlock.TryParseRegex(pattern, out var match)) return 0;

        return int.TryParse(match.Groups[1].Value, out var result) ? result : 0;
    }

    protected static int GetInt(Regex pattern, string value)
    {
        var match = pattern.Match(value);
        if (!match.Success) return 0;

        return int.TryParse(match.Groups[1].Value, out var result) ? result : 0;
    }

    protected static double GetDouble(Regex pattern, OriginalText originalText)
    {
        if (!originalText.TryParseRegex(pattern, out var match)) return 0;

        var value = match.Groups[1].Value
            .Replace(",", ".")
            .TrimEnd('%');

        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        return 0;
    }
}
