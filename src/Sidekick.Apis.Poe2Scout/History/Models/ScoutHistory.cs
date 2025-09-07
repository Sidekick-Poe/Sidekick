namespace Sidekick.Apis.Poe2Scout.History.Models;

public class ScoutHistory
{
    public required string? Category { get; set; }

    public List<ScoutHistoryLog> Exalted { get; set; } = [];
    public List<ScoutHistoryLog> Chaos { get; set; } = [];
    public List<ScoutHistoryLog> Divine { get; set; } = [];
}
