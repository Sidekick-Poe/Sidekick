using System.Globalization;
using System.Text.RegularExpressions;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Parser.Properties;

public abstract class PropertyDefinition
{
    private static readonly Regex parseHashPattern = new("\\#");

    public abstract bool Enabled { get; }

    public abstract void Initialize();

    public abstract void ParseBeforeModifiers(ItemProperties itemProperties, ParsingItem parsingItem);

    public abstract void ParseAfterModifiers(ItemProperties itemProperties, ParsingItem parsingItem, List<ModifierLine> modifierLines);

    protected static bool GetBool(Regex? pattern, ParsingItem parsingItem)
    {
        if (pattern == null)
        {
            return false;
        }

        return parsingItem.TryParseRegex(pattern, out _);
    }

    protected static int GetInt(Regex? pattern, ParsingItem parsingItem)
    {
        if (pattern == null)
        {
            return 0;
        }

        if (parsingItem.TryParseRegex(pattern, out var match) && int.TryParse(match.Groups[1].Value, out var result))
        {
            return result;
        }

        return 0;
    }

    protected static int GetInt(Regex? pattern, ParsingBlock parsingBlock)
    {
        if (pattern == null)
        {
            return 0;
        }

        if (parsingBlock.TryParseRegex(pattern, out var match) && int.TryParse(match.Groups[1].Value, out var result))
        {
            return result;
        }

        return 0;
    }

    protected static double GetDouble(Regex? pattern, ParsingBlock parsingBlock)
    {
        if (pattern == null)
        {
            return 0;
        }

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
