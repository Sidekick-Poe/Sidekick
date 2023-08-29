namespace Sidekick.Apis.Poe.Trade.Results
{
    public class LogbookMod
    {
        public string? Name { get; set; }
        public List<string> Mods { get; set; } = new();
        public Faction? Faction { get; set; }
    }
}
