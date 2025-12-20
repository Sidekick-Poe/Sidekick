namespace Sidekick.Apis.Poe.Trade.Trade.Items.Models;

public class LogbookMod
{
    public required string Name { get; init; }
    public List<string> Mods { get; init; } = new();
    public required LogbookFaction Faction { get; init; }
}
