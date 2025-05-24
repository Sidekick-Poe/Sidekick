namespace Sidekick.Apis.Poe.Account.Stash.Models;

public class ApiStashTab
{
    public required string Id { get; set; }
    public string? Parent { get; set; }
    public required string Name { get; set; }
    public string? Type { get; set; }
    public List<ApiStashTab>? Children { get; set; }
    public List<APIStashItem>? Items { get; set; }
    public APIStashMetadata? Metadata { get; set; }

    public StashType StashType => Type switch
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
