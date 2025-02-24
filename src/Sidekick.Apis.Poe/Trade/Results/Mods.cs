using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Results;

public class Mods
{
    public List<Mod> Implicit { get; set; } = [];
    public List<Mod> Explicit { get; set; } = [];
    public List<Mod> Crafted { get; set; } = [];
    public List<Mod> Enchant { get; set; } = [];
    public List<Mod> Rune { get; set; } = [];
    public List<Mod> Fractured { get; set; } = [];
    public List<Mod> Scourge { get; set; } = [];
    public List<Mod> Sanctum { get; set; } = [];

    [JsonIgnore]
    public List<Mod> Pseudo { get; set; } = [];
}
