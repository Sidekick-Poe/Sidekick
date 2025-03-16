using System.Text.Json.Serialization;
using Sidekick.Common.Converters;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Trade.Results;

public class ResultItem
{
    public string? Name { get; set; }

    public string? TypeLine { get; set; }

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

    public List<ResultLineContent> Requirements { get; set; } = new();

    public List<ResultLineContent> Properties { get; set; } = new();

    public List<ResultLineContent> AdditionalProperties { get; set; } = new();

    [JsonPropertyName("implicitMods")]
    public List<string> ImplicitMods { get; set; } = new();

    [JsonPropertyName("craftedMods")]
    public List<string> CraftedMods { get; set; } = new();

    [JsonPropertyName("explicitMods")]
    public List<string> ExplicitMods { get; set; } = new();

    [JsonPropertyName("utilityMods")]
    public List<string> UtilityMods { get; set; } = new();

    [JsonPropertyName("pseudoMods")]
    public List<string> PseudoMods { get; set; } = new();

    [JsonPropertyName("enchantMods")]
    public List<string> EnchantMods { get; set; } = new();

    [JsonPropertyName("runeMods")]
    public List<string> RuneMods { get; set; } = new();

    [JsonPropertyName("fracturedMods")]
    public List<string> FracturedMods { get; set; } = new();

    [JsonPropertyName("scourgeMods")]
    public List<string> ScourgeMods { get; set; } = new();

    [JsonPropertyName("sanctumMods")]
    public List<string> SanctumMods { get; set; } = new();

    public List<string> GemSockets { get; set; } = new();

    public List<ResultSocket> Sockets { get; set; } = new();

    [JsonPropertyName("extended")]
    [JsonConverter(typeof(ObjectOrArrayConverter<Extended>))]
    public Extended? Extended { get; set; }

    public List<LogbookMod> LogbookMods { get; set; } = new();
}
