using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Sidekick.Game.BaseItems;

namespace Sidekick.Game.ItemDefinitions;

public class ItemDefinition
{
    public List<string>? UniqueIds { get; set; }

    public List<string>? BaseItemIds { get; set; }

    public string? ExchangeId { get; set; }

    public string? Name { get; init; }

    public string? Image { get; init; }

    public NinjaExchangeItem? NinjaExchange { get; init; }

    public List<TradeItem>? TradeItems { get; init; }

    public List<NinjaStashItem>? NinjaItems { get; init; }

    [JsonIgnore]
    public Regex? NamePattern { get; init; }

    [JsonPropertyName("namePattern")]
    public string? NamePatternValue
    {
        get { return NamePattern?.ToString(); }
        init { NamePattern = value == null ? null : new Regex(value); }
    }

    [JsonIgnore]
    public Regex? TypePattern { get; init; }

    [JsonPropertyName("typePattern")]
    public string? TypePatternValue
    {
        get { return TypePattern?.ToString(); }
        init { TypePattern = value == null ? null : new Regex(value); }
    }

    [JsonIgnore]
    public bool IsUnique => UniqueIds?.Any() ?? false;

    [JsonIgnore]
    public List<BaseItemDefinition> BaseItems { get; set; } = [];

    /// <inheritdoc />
    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}