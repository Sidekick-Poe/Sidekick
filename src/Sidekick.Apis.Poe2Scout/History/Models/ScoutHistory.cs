namespace Sidekick.Apis.Poe2Scout.History.Models;

public class ScoutHistory
{
    public List<ScoutHistoryLog>? Exalted { get; init; }
    public List<ScoutHistoryLog>? Chaos { get; init; }
    public List<ScoutHistoryLog>? Divine { get; init; }
}
