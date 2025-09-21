using System.Globalization;
using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties;

public abstract class PropertyDefinition
{
    public abstract List<Category> ValidCategories { get; }

    public virtual void Parse(ItemProperties itemProperties, ParsingItem parsingItem, ItemHeader header) { }

    public virtual void ParseAfterModifiers(ItemProperties itemProperties, ParsingItem parsingItem, List<ModifierLine> modifierLines) { }

    public virtual Task<PropertyFilter?> GetFilter(Item item, double normalizeValue, FilterType filterType) { return Task.FromResult<PropertyFilter?>(null); }

    public virtual List<PropertyFilter>? GetFilters(Item item, double normalizeValue, FilterType filterType) { return null; }

    public abstract void PrepareTradeRequest(Query query, Item item, PropertyFilter filter);

    protected static bool GetBool(Regex pattern, ParsingItem parsingItem) => parsingItem.TryParseRegex(pattern, out _);

    protected static bool GetBool(Regex pattern, ParsingBlock parsingBlock) => parsingBlock.TryParseRegex(pattern, out _);

    protected static string? GetString(Regex pattern, ParsingBlock parsingBlock)
    {
        if (parsingBlock.TryParseRegex(pattern, out var match))
        {
            return match.Groups[1].Value.Trim(' ', ':');
        }

        return null;
    }

    protected static int GetInt(Regex pattern, ParsingItem parsingItem)
    {
        if (parsingItem.TryParseRegex(pattern, out var match) && int.TryParse(match.Groups[1].Value, out var result))
        {
            return result;
        }

        return 0;
    }

    protected static int GetInt(Regex pattern, ParsingBlock parsingBlock)
    {
        if (parsingBlock.TryParseRegex(pattern, out var match) && int.TryParse(match.Groups[1].Value, out var result))
        {
            return result;
        }

        return 0;
    }

    protected static double GetDouble(Regex pattern, ParsingBlock parsingBlock)
    {
        if (!parsingBlock.TryParseRegex(pattern, out var match))
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
