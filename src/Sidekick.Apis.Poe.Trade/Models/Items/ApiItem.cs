using System.Text.Json.Serialization;
using Sidekick.Apis.Poe.Items;
using Sidekick.Common.Converters;

namespace Sidekick.Apis.Poe.Trade.Models.Items;

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

    public string? Note { get; set; }

    public List<ApiItemLineContent> Requirements { get; set; } = [];

    public List<ApiItemLineContent> Properties { get; set; } = [];

    public List<ApiItemLineContent> AdditionalProperties { get; set; } = [];

    [JsonPropertyName("implicitMods")]
    public List<string> ImplicitMods { get; set; } = [];

    [JsonPropertyName("craftedMods")]
    public List<string> CraftedMods { get; set; } = [];

    [JsonPropertyName("explicitMods")]
    public List<string> ExplicitMods { get; set; } = [];

    [JsonPropertyName("utilityMods")]
    public List<string> UtilityMods { get; set; } = [];

    [JsonPropertyName("pseudoMods")]
    public List<string> PseudoMods { get; set; } = [];

    [JsonPropertyName("enchantMods")]
    public List<string> EnchantMods { get; set; } = [];

    [JsonPropertyName("runeMods")]
    public List<string> RuneMods { get; set; } = [];

    [JsonPropertyName("fracturedMods")]
    public List<string> FracturedMods { get; set; } = [];

    [JsonPropertyName("desecratedMods")]
    public List<string> DesecratedMods { get; set; } = [];

    [JsonPropertyName("scourgeMods")]
    public List<string> ScourgeMods { get; set; } = [];

    [JsonPropertyName("sanctumMods")]
    public List<string> SanctumMods { get; set; } = [];

    [JsonPropertyName("mutatedMods")]
    public List<string> MutatedMods { get; set; } = [];

    public List<string> GemSockets { get; set; } = [];

    public List<ApiItemSocket> Sockets { get; set; } = [];

    [JsonPropertyName("extended")]
    [JsonConverter(typeof(ObjectOrArrayConverter<Extended>))]
    public Extended? Extended { get; set; }

    public List<LogbookMod> LogbookMods { get; set; } = [];

    [JsonPropertyName("grantedSkills")]
    public List<ApiItemLineContent> GrantedSkills { get; set; } = [];

    [JsonIgnore]
    public int? MaxLinks
    {
        get
        {
            if (Sockets.Count == 0) return null;
            return Sockets.GroupBy(x => x.Group).Max(x => x.Key);
        }
    }

    [JsonIgnore]
    public int? GemLevel => GetPropertyValue("Level", 1);

    [JsonIgnore]
    public int? MapTier => GetPropertyValue("Map Tier", 16);

    private int? GetPropertyValue(string name, int defaultValue = 0)
    {
        var property = Properties.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
        if (property == null) return null;

        var value = property.Values.FirstOrDefault()?.FirstOrDefault();
        if (value == null) return defaultValue;

        return value.Value.GetInt32();
    }

    public bool HasModifiers => !Identified || ImplicitMods.Count > 0 || CraftedMods.Count > 0 || ExplicitMods.Count > 0 || UtilityMods.Count > 0 || PseudoMods.Count > 0 || EnchantMods.Count > 0 || RuneMods.Count > 0 || FracturedMods.Count > 0 || DesecratedMods.Count > 0 || ScourgeMods.Count > 0 || SanctumMods.Count > 0 || LogbookMods.Count > 0 || MutatedMods.Count > 0;
}
