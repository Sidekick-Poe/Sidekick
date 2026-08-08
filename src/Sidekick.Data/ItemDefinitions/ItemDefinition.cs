using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
namespace Sidekick.Data.ItemDefinitions;

public class ItemDefinition
{
    public string? InvariantKey { get; init; }

    public bool IsUnique => InvariantKey?.StartsWith("UNIQUE_") ?? false;

    public string? ItemClassId { get; init; }

    public string? Name { get; init; }

    public string? Image { get; init; }

    public BaseItemProperties? Properties { get; set; }

    public BaseItemRequirements? Requirements { get; set; }

    public ExchangeItemDefinition? ExchangeItem { get; init; }

    public List<TradeItemDefinition>? TradeItems { get; init; }

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

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Name} ({ItemClassId})";
    }
}
