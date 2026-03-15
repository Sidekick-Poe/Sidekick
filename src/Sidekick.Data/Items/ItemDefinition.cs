using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
namespace Sidekick.Data.Items;

public class ItemDefinition
{
    public string? Id { get; init; }
    public string? Text { get; init; }
    public string? Image { get; init; }

    public string? InvariantText { get; set; }
    public string? InvariantName { get; set; }
    public string? InvariantType { get; set; }

    public string? Name { get; init; }
    public string? Type { get; init; }
    public string? Category { get; init; }
    public string? Discriminator { get; init; }
    public bool IsUnique { get; init; }

    [JsonIgnore]
    public Regex? NamePattern { get; set; }

    [JsonPropertyName("namePattern")]
    public string? NamePatternValue
    {
        get
        {
            return NamePattern?.ToString();
        }
        set
        {
            NamePattern = value == null ? null : new Regex(value);
        }
    }

    [JsonIgnore]
    public Regex? TypePattern { get; set; }

    [JsonPropertyName("typePattern")]
    public string? TypePatternValue
    {
        get
        {
            return TypePattern?.ToString();
        }
        set
        {
            TypePattern = value == null ? null : new Regex(value);
        }
    }

    [JsonIgnore]
    public Regex? TextPattern { get; set; }

    [JsonPropertyName("textPattern")]
    public string? TextPatternValue
    {
        get
        {
            return TextPattern?.ToString();
        }
        set
        {
            TextPattern = value == null ? null : new Regex(value);
        }
    }

    /// <inheritdoc />
    public override string? ToString()
    {
        if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Name))
        {
            return $"{Type} - {Name}";
        }

        return !string.IsNullOrEmpty(Type) ? Type : Name;
    }
}
