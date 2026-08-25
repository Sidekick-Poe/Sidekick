using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Sidekick.Game.BaseItems;
using Sidekick.Game.Ninja;
using Sidekick.Game.Scout;

namespace Sidekick.Game.ItemDefinitions;

public class ItemDefinition
{
    public List<string>? UniqueIds { get; set; }

    public List<string>? BaseItemIds { get; set; }

    [JsonPropertyName("ninjaExchange")]
    public string? NinjaExchangeItemId { get; set; }

    [JsonPropertyName("ninjaStash")]
    public List<string>? NinjaStashItemIds { get; set; }

    [JsonPropertyName("scout")]
    public List<int>? ScoutItemIds { get; set; }

    public string? ExchangeId { get; set; }

    public string? Name { get; init; }

    public string? Image { get; init; }

    public List<TradeItem>? TradeItems { get; init; }

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

    [JsonIgnore]
    public NinjaExchangeItem? NinjaExchangeItem { get; set; }

    [JsonIgnore]
    public List<NinjaStashItem>? NinjaStashItems { get; set; }

    [JsonIgnore]
    public List<ScoutItem>? ScoutItems { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}