namespace Sidekick.Apis.Poe.Trade.Models;

public class LogbookMod
{
    public required string Name { get; init; }
    public List<ApiItemModifier> Mods { get; init; } = new();
    public required LogbookFaction Faction { get; init; }
}
