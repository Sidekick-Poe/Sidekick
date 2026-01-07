using System.Globalization;
using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public abstract class PropertyDefinition
{
    public abstract List<ItemClass> ValidItemClasses { get; }

    public abstract string Label { get; }

    public virtual void Parse(Item item) {}

    public virtual void ParseAfterStats(Item item) {}

    public virtual Task<TradeFilter?> GetFilter(Item item) { return Task.FromResult<TradeFilter?>(null); }

    protected static bool GetBool(Regex pattern, TextItem textItem) => textItem.TryParseRegex(pattern, out _);

    protected static bool GetBool(Regex pattern, TextBlock textBlock) => textBlock.TryParseRegex(pattern, out _);

    protected static string? GetString(Regex pattern, TextBlock textBlock)
    {
        if (textBlock.TryParseRegex(pattern, out var match))
        {
            return match.Groups[1].Value.Trim(' ', ':');
        }

        return null;
    }

    protected static int GetInt(Regex pattern, TextItem textItem)
    {
        if (textItem.TryParseRegex(pattern, out var match) && int.TryParse(match.Groups[1].Value, out var result))
        {
            return result;
        }

        return 0;
    }

    protected static int GetInt(Regex pattern, TextBlock textBlock)
    {
        if (!textBlock.TryParseRegex(pattern, out var match)) return 0;

        return int.TryParse(match.Groups[1].Value, out var result) ? result : 0;
    }

    protected static double GetDouble(Regex pattern, TextBlock textBlock)
    {
        if (!textBlock.TryParseRegex(pattern, out var match))
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
