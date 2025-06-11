namespace Sidekick.Apis.Poe.Trade.Models.Items;

public class LogbookMod
{
    public required string Name { get; init; }
    public List<string> Mods { get; init; } = new();
    public required LogbookFaction Faction { get; init; }
}
