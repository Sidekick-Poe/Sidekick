namespace Sidekick.Apis.Poe.Trade.Trade.Results;

public class LogbookMod
{
    public required string Name { get; init; }
    public List<string> Mods { get; init; } = new();
    public required Faction Faction { get; init; }
}
