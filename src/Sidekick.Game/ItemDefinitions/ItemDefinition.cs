using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Sidekick.Game.ItemDefinitions;

public class ItemDefinition
{
    public string? InvariantKey { get; init; }

    public string? ItemClassId { get; init; }

    public string? Name { get; init; }

    public string? Image { get; init; }

    public BaseItemProperties? Properties { get; set; }

    public BaseItemRequirements? Requirements { get; set; }

    public ExchangeItem? Exchange { get; init; }

    public NinjaExchangeItem? NinjaExchange { get; init; }

    public List<TradeItemDefinition>? TradeItems { get; init; }

    public List<NinjaStashItem>? NinjaItems { get; init; }

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
    public bool IsUnique => InvariantKey?.StartsWith("UNIQUE_") ?? false;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Name} ({ItemClassId})";
    }
}
