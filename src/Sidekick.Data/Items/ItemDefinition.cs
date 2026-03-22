using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
namespace Sidekick.Data.Items;

public class ItemDefinition
{
    public TradeItemDefinition? TradeItem { get; init; }

    public BaseItemDefinition? BaseItem { get; init; }

    public UniqueItemDefinition? UniqueItem { get; init; }

    [JsonIgnore]
    public Regex? NamePattern { get; init; }

    [JsonPropertyName("namePattern")]
    public string? NamePatternValue
    {
        get
        {
            return NamePattern?.ToString();
        }
        init
        {
            NamePattern = value == null ? null : new Regex(value);
        }
    }

    [JsonIgnore]
    public Regex? TypePattern { get; init; }

    [JsonPropertyName("typePattern")]
    public string? TypePatternValue
    {
        get
        {
            return TypePattern?.ToString();
        }
        init
        {
            TypePattern = value == null ? null : new Regex(value);
        }
    }

    [JsonIgnore]
    public Regex? TextPattern { get; init; }

    [JsonPropertyName("textPattern")]
    public string? TextPatternValue
    {
        get
        {
            return TextPattern?.ToString();
        }
        init
        {
            TextPattern = value == null ? null : new Regex(value);
        }
    }

    /// <inheritdoc />
    public override string? ToString()
    {
        var type = TradeItem?.Type ?? BaseItem?.Name;
        var name = TradeItem?.Name;

        if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(name))
        {
            return $"{type} - {name}";
        }

        return !string.IsNullOrEmpty(type) ? type : name;
    }
}