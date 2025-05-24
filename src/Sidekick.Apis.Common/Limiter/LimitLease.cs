using System.Threading.RateLimiting;

namespace Sidekick.Apis.Common.Limiter;

public class LimitLease :IDisposable
{
    public required List<RateLimitLease> Leases { get; init;  }

    public required RateLimitLease ConcurrencyLease { get; init; }

    public void Dispose()
    {
        foreach (var lease in Leases)
        {
            lease.Dispose();
        }

        ConcurrencyLease.Dispose();
    }
}
