using System.Text.Json.Serialization;
using Sidekick.Common.Converters;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Trade.Trade.Results;

public class TradeItem
{
    public string? Name { get; set; }

    [JsonPropertyName("typeLine")]
    public string? Type { get; set; }

    public bool Identified { get; set; }

    [JsonPropertyName("ilvl")]
    public int ItemLevel { get; set; }

    [JsonPropertyName("frameType")]
    public Rarity Rarity { get; set; }

    [JsonPropertyName("foilVariation")]
    public int? FoilVariation { get; set; }

    public bool Corrupted { get; set; }

    public Scourged Scourged { get; set; } = new Scourged();

    public bool Fractured { get; set; }

    public bool IsRelic { get; set; }

    public Influences Influences { get; set; } = new Influences();

    public bool Verified { get; set; }

    [JsonPropertyName("w")]
    public int Width { get; set; }

    [JsonPropertyName("h")]
    public int Height { get; set; }

    public string? Icon { get; set; }

    public string? League { get; set; }

    public string? Note { get; set; }

    public List<ResultLineContent> Requirements { get; set; } = [];

    public List<ResultLineContent> Properties { get; set; } = [];

    public List<ResultLineContent> AdditionalProperties { get; set; } = [];

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

    [JsonPropertyName("scourgeMods")]
    public List<string> ScourgeMods { get; set; } = [];

    [JsonPropertyName("sanctumMods")]
    public List<string> SanctumMods { get; set; } = [];

    public List<string> GemSockets { get; set; } = [];

    public List<ResultSocket> Sockets { get; set; } = [];

    [JsonPropertyName("extended")]
    [JsonConverter(typeof(ObjectOrArrayConverter<Extended>))]
    public Extended? Extended { get; set; }

    public List<LogbookMod> LogbookMods { get; set; } = [];

    [JsonPropertyName("grantedSkills")]
    public List<ResultLineContent> GrantedSkills { get; set; } = [];

    public bool HasModifiers => !Identified || ImplicitMods.Count > 0 || CraftedMods.Count > 0 || ExplicitMods.Count > 0 || UtilityMods.Count > 0 || PseudoMods.Count > 0 || EnchantMods.Count > 0 || RuneMods.Count > 0 || FracturedMods.Count > 0 || ScourgeMods.Count > 0 || SanctumMods.Count > 0 || LogbookMods.Count > 0;
}
