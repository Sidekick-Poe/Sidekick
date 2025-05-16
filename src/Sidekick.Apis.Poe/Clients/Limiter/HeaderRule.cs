namespace Sidekick.Apis.Poe.Clients.Limiter;

internal class HeaderRule
(
    string name,
    int currentHitCount,
    int maxHitCount,
    int timePeriod
)
{
    public string Name { get; init; } = name;

    public int CurrentHitCount { get; init; } = currentHitCount;

    public int MaxHitCount { get; init; } = maxHitCount;

    public int TimePeriod { get; init; } = timePeriod;
}
