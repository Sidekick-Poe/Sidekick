namespace Sidekick.Apis.Poe.Clients.Limiter;

internal class HeaderRule
{
    public HeaderRule(string name, int currentHitCount, int maxHitCount, int timePeriod)
    {
        Name = name;
        CurrentHitCount = currentHitCount;
        MaxHitCount = maxHitCount;
        TimePeriod = timePeriod;
    }

    public string Name { get; init; }

    public int CurrentHitCount { get; init; }

    public int MaxHitCount { get; init; }

    public int TimePeriod { get; init; }
}
