using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Stash.Models;

public class ApiStashTabList
{
    [JsonPropertyName("stashes")]
    public required List<ApiStashTab> StashTabs { get; set; }
}
