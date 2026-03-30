using System.Globalization;
using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Data.Items;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public abstract class PropertyDefinition
{
    public abstract string Label { get; }

    public virtual void Parse(Item item) {}

    public virtual void ParseAfterStats(Item item) {}

    public virtual Task<TradeFilter?> GetFilter(Item item) { return Task.FromResult<TradeFilter?>(null); }

    protected static bool GetBool(Regex pattern, RawText rawText) => rawText.TryParseRegex(pattern, out _);

    protected static bool GetBool(Regex pattern, RawBlock rawBlock) => rawBlock.TryParseRegex(pattern, out _);

    protected static string? GetString(Regex pattern, RawBlock rawBlock)
    {
        if (rawBlock.TryParseRegex(pattern, out var match))
        {
            return match.Groups[1].Value.Trim(' ', ':');
        }

        return null;
    }

    protected static int GetInt(Regex pattern, RawText rawText)
    {
        if (rawText.TryParseRegex(pattern, out var match) && int.TryParse(match.Groups[1].Value, out var result))
        {
            return result;
        }

        return 0;
    }

    protected static int GetInt(Regex pattern, RawBlock rawBlock)
    {
        if (!rawBlock.TryParseRegex(pattern, out var match)) return 0;

        return int.TryParse(match.Groups[1].Value, out var result) ? result : 0;
    }

    protected static int GetInt(Regex pattern, string value)
    {
        var match = pattern.Match(value);
        if (!match.Success) return 0;

        return int.TryParse(match.Groups[1].Value, out var result) ? result : 0;
    }

    protected static double GetDouble(Regex pattern, RawBlock rawBlock)
    {
        if (!rawBlock.TryParseRegex(pattern, out var match))
        {
            return 0;
        }

        var value = match.Groups[1].Value.Replace(",", ".");
        if (value.EndsWith("%"))
        {
            value = value.TrimEnd('%');
        }

        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        return 0;
    }
}
