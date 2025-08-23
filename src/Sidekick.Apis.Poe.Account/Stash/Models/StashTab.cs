using System.Text.Json.Serialization;
using Sidekick.Apis.Poe.Trade.Models.Items;

namespace Sidekick.Apis.Poe.Account.Stash.Models;

public class StashTab
{
    public required string Id { get; init; }

    public string? Parent { get; init; }

    public required string Name { get; init; }

    [JsonPropertyName("type")]
    public string? TypeLine { get; set; }

    public List<StashTab>? Children { get; set; }

    public List<ApiItem>? Items { get; set; }

    public StashMetadata? Metadata { get; set; }

    [JsonIgnore]
    public StashType Type => TypeLine switch
    {
        "CurrencyStash" => StashType.Currency,
        "EssenceStash" => StashType.Essences,
        "Folder" => StashType.Folder,
        "MetamorphStash" => StashType.Metamorph,
        "DelveStash" => StashType.Delve,
        "MapStash" => StashType.Map,
        "BlightStash" => StashType.Blight,
        "FragmentStash" => StashType.Fragment,
        "DeliriumStash" => StashType.Delirium,
        "DivinationCardStash" => StashType.DivinationCard,
        "FlaskStash" => StashType.Flask,
        "GemStash" => StashType.Gem,
        "UniqueStash" => StashType.Unique,
        "PremiumStash" => StashType.Premium,
        "QuadStash" => StashType.Quad,
        _ => StashType.Unknown,
    };
}
