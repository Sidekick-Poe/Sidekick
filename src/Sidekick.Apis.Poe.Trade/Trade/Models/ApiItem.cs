using Sidekick.Apis.Poe.Trade.Trade.Converters;
using Sidekick.Common.Converters;
using Sidekick.Game.Items;
using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe.Trade.Trade.Models;

public class ApiItem
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    [JsonPropertyName("typeLine")]
    public string? TypeLine { get; set; }

    [JsonPropertyName("baseType")]
    public string? BaseType { get; set; }

    public string? Type => TypeLine ?? BaseType;

    public bool Identified { get; set; }

    [JsonPropertyName("ilvl")]
    public int ItemLevel { get; set; }

    [JsonPropertyName("frameType")]
    public Rarity Rarity { get; set; }

    [JsonPropertyName("foilVariation")]
    public int? FoilVariation { get; set; }

    public bool Corrupted { get; set; }

    public bool Split { get; set; }

    public bool Mutated { get; set; }

    public ApiItemScourged Scourged { get; set; } = new();

    public bool Fractured { get; set; }

    public bool Sanctified { get; set; }

    [JsonPropertyName("duplicated")]
    public bool Mirrored { get; set; }

    public bool IsRelic { get; set; }

    public Influences Influences { get; set; } = new();

    public bool Verified { get; set; }

    [JsonPropertyName("w")]
    public int Width { get; set; }

    [JsonPropertyName("h")]
    public int Height { get; set; }

    public int? StackSize { get; set; }

    public string? Icon { get; set; }

    public string? SocketedIcon { get; set; }

    public string? Note { get; set; }

    public string? BuiltInSupport { get; set; }

    public List<ApiItemLineContent> Requirements { get; set; } = [];

    public List<ApiItemLineContent> Properties { get; set; } = [];

    public List<ApiItemLineContent> AdditionalProperties { get; set; } = [];

    [JsonPropertyName("utilityMods")]
    public List<string> UtilityMods { get; set; } = [];

    [JsonPropertyName("pseudoMods")]
    public List<string> PseudoMods { get; set; } = [];

    [JsonPropertyName("enchantMods")]
    [JsonConverter(typeof(StringOrModifierListConverter))]
    public List<ApiItemModifier> EnchantMods { get; set; } = [];

    [JsonPropertyName("runeMods")]
    [JsonConverter(typeof(StringOrModifierListConverter))]
    public List<ApiItemModifier> RuneMods { get; set; } = [];

    [JsonPropertyName("implicitMods")]
    [JsonConverter(typeof(StringOrModifierListConverter))]
    public List<ApiItemModifier> ImplicitMods { get; set; } = [];

    [JsonPropertyName("explicitMods")]
    [JsonConverter(typeof(StringOrModifierListConverter))]
    public List<ApiItemModifier> ExplicitMods { get; set; } = [];

    [JsonPropertyName("veiledMods")]
    [JsonConverter(typeof(StringOrModifierListConverter))]
    public List<ApiItemModifier> VeiledMods { get; set; } = [];

    public List<string> GemSockets { get; set; } = [];

    [JsonPropertyName("socketedItems")]
    public List<ApiItem> SocketedItems { get; set; } = [];

    public List<ApiItemSocket> Sockets { get; set; } = [];

    [JsonPropertyName("extended")]
    [JsonConverter(typeof(ObjectOrArrayConverter<Extended>))]
    public Extended? Extended { get; set; }

    [JsonPropertyName("logbookMods")]
    public List<LogbookMod> LogbookMods { get; set; } = [];

    [JsonPropertyName("grantedSkills")]
    public List<ApiItemLineContent> GrantedSkills { get; set; } = [];

    [JsonIgnore]
    public int? MaxLinks
    {
        get
        {
            if (Sockets.Count == 0) return null;
            return Sockets.GroupBy(x => x.Group).Max(x => x.Count());
        }
    }

    [JsonIgnore]
    public int? GemLevel => GetPropertyValue("Level", 1);

    [JsonIgnore]
    public int? Quality => GetPropertyValue("Quality");

    [JsonIgnore]
    public int? MapTier => GetPropertyValue("Map Tier", 16);

    private int? GetPropertyValue(string name, int defaultValue = 0)
    {
        var property = Properties.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
        if (property == null) return null;

        var value = property.Values.FirstOrDefault()?.FirstOrDefault();
        if (value == null) return defaultValue;

        var stringValue = value.Value.GetString();
        if (string.IsNullOrWhiteSpace(stringValue)) return defaultValue;

        var numericPart = stringValue.Trim('-', '+', '%').Split(' ')[0];
        if (int.TryParse(numericPart, out var intValue)) return intValue;

        return defaultValue;
    }

    public bool HasStats => !Identified ||
                            ImplicitMods.Count > 0 ||
                            ExplicitMods.Count > 0 ||
                            UtilityMods.Count > 0 ||
                            PseudoMods.Count > 0 ||
                            EnchantMods.Count > 0 ||
                            RuneMods.Count > 0 ||
                            VeiledMods.Count > 0 ||
                            LogbookMods.Count > 0 ||
                            Split ||
                            Fractured ||
                            Mirrored;
}
