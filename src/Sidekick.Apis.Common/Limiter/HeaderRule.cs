namespace Sidekick.Apis.Common.Limiter;

internal class HeaderRule
(
    string policy,
    string name,
    int currentHitCount,
    int maxHitCount,
    int timePeriod
)
{
    public string Policy { get; init; } = policy;

    public string Name { get; init; } = name;

    public int CurrentHitCount { get; init; } = currentHitCount;

    public int MaxHitCount { get; init; } = maxHitCount;

    public int TimePeriod { get; init; } = timePeriod;
}
