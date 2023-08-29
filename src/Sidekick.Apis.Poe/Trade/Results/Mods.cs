using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Results
{
    public class Mods
    {
        public List<Mod> Implicit { get; set; } = new();
        public List<Mod> Explicit { get; set; } = new();
        public List<Mod> Crafted { get; set; } = new();
        public List<Mod> Enchant { get; set; } = new();
        public List<Mod> Fractured { get; set; } = new();
        public List<Mod> Scourge { get; set; } = new();

        [JsonIgnore]
        public List<Mod> Pseudo { get; set; } = new();
    }
}
