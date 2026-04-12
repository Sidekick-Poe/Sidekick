using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
namespace Sidekick.Data.ItemDefinitions;

public class ItemDefinition
{
    [JsonIgnore]
    public string Key
    {
        get
        {
            var key = new StringBuilder();
            if (!string.IsNullOrEmpty(UniqueItem?.Id)) key.Append(UniqueItem.Id);
            if (!string.IsNullOrEmpty(TradeItem?.Id)) key.Append(TradeItem.Id);
            if (!string.IsNullOrEmpty(TradeItem?.Discriminator)) key.Append(TradeItem.Discriminator);
            if (!string.IsNullOrEmpty(BaseItem?.Id)) key.Append(BaseItem.Id);

            return key.ToString();
        }
    }

    public TradeItemDefinition? TradeItem { get; init; }

    public BaseItemDefinition? BaseItem { get; init; }

    public UniqueItemDefinition? UniqueItem { get; init; }

    public List<NinjaItemDefinition>? NinjaItems { get; init; }

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
    public override string ToString()
    {
        var type = TradeItem?.Type ?? BaseItem?.Name;
        var name = TradeItem?.Name;
        return $"{name} {type}".Trim();
    }
}