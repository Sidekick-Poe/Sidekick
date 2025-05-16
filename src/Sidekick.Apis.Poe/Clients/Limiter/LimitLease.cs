using System.Threading.RateLimiting;

namespace Sidekick.Apis.Poe.Clients.Limiter;

internal class LimitLease :IDisposable
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
