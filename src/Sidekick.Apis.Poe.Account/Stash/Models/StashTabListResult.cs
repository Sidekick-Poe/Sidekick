using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Account.Stash.Models;

public class StashTabListResult
{
    [JsonPropertyName("stashes")]
    public required List<StashTab> Tabs { get; set; }
}
