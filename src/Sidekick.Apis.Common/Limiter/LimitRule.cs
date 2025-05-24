using System.Threading.RateLimiting;

namespace Sidekick.Apis.Common.Limiter;

internal class LimitRule
{
    public LimitRule(string name, int maxHitCount, int timePeriod)
    {
        Name = name;
        MaxHitCount = maxHitCount;
        TimePeriod = timePeriod;

        Limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions()
        {
            AutoReplenishment = false,
            QueueLimit = int.MaxValue,
            TokenLimit = MaxHitCount,
            TokensPerPeriod = MaxHitCount,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            ReplenishmentPeriod = TimeSpan.FromSeconds(timePeriod + 5),
        });
    }

    public string Name { get; }

    public int MaxHitCount { get; }

    public int TimePeriod { get; }

    public TokenBucketRateLimiter Limiter { get; }

    private Timer? ReplenishTimer { get; set; }

    public async Task HandleResponse(int currentHitCount)
    {
        if (currentHitCount == 1 && ReplenishTimer != null)
        {
            await ReplenishTimer.DisposeAsync();
            ReplenishTimer = null;
        }

        ReplenishTimer ??= new Timer(static state =>
                                     {
                                         var self = (LimitRule?)state;
                                         if (self == null)
                                         {
                                             return;
                                         }

                                         self.ReplenishTimer?.Dispose();
                                         self.ReplenishTimer = null;
                                         self.Limiter.TryReplenish();
                                     },
                                     this,
                                     TimeSpan.FromSeconds(TimePeriod + 5),
                                     Timeout.InfiniteTimeSpan);
    }
}
