using Sidekick.Game.Scout;
namespace Sidekick.Apis.Poe2Scout.History.Models;

public class ScoutHistory
{
    public required ScoutItem Item { get; init; }
    public List<ScoutHistoryLog>? Exalted { get; init; }
    public List<ScoutHistoryLog>? Chaos { get; init; }
    public List<ScoutHistoryLog>? Divine { get; init; }
}
