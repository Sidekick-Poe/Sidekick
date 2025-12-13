using System.Threading.RateLimiting;

namespace Sidekick.Apis.Common.Limiter;

public class LimitRule(string policy, string name, int maxHitCount, int timePeriod)
    : IDisposable, IAsyncDisposable
{
    public string Policy { get; } = policy;

    public string Name { get; } = name;

    public int MaxHitCount { get; } = maxHitCount;

    public int TimePeriod { get; } = timePeriod;

    public ServerSynchronizedRateLimiter Limiter { get; } = new(maxHitCount,
                                                                TimeSpan.FromSeconds(timePeriod));

    public int CurrentHitCount => Limiter.CurrentHitCount;

    public int Used => Math.Clamp(CurrentHitCount, 0, MaxHitCount);

    public double Ratio => Used / (double)MaxHitCount;

    public override string ToString()
    {
        return $"{Name} - {Policy} - {MaxHitCount} in {TimePeriod} seconds";
    }

    public void Dispose()
    {
        Limiter.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Limiter.DisposeAsync();
    }
}
