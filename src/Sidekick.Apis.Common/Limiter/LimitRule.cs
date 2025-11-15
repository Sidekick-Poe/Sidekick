using System.Threading.RateLimiting;

namespace Sidekick.Apis.Common.Limiter;

public class LimitRule
{
    private readonly Action? onChanged;

    public LimitRule(string policy, string name, int maxHitCount, int timePeriod, Action? onChanged = null)
    {
        Policy = policy;
        Name = name;
        MaxHitCount = maxHitCount;
        TimePeriod = timePeriod;
        this.onChanged = onChanged;

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

    public string Policy { get; }

    public string Name { get; }

    public int MaxHitCount { get; }

    public int TimePeriod { get; }

    public TokenBucketRateLimiter Limiter { get; }

    private int CurrentHitCount { get; set; }

    private Timer? ReplenishTimer { get; set; }

    public int Used => Math.Clamp(CurrentHitCount, 0, MaxHitCount);

    public int Available => Math.Max(0, MaxHitCount - Used);

    public double Ratio => Used / (double)MaxHitCount;

    internal async Task HandleResponse(int currentHitCount)
    {
        CurrentHitCount = currentHitCount;
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
                                         // Window elapsed: reset the hit count and replenish tokens, then notify listeners
                                         self.CurrentHitCount = 0;
                                         self.Limiter.TryReplenish();
                                         self.onChanged?.Invoke();
                                     },
                                     this,
                                     TimeSpan.FromSeconds(TimePeriod + 5),
                                     Timeout.InfiniteTimeSpan);
    }
}
