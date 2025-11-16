using System.Threading.RateLimiting;
namespace Sidekick.Apis.Common.Limiter;

public sealed class ServerSynchronizedRateLimitLease : RateLimitLease
{
    private const string RetryAfterName = "RETRY_AFTER";

    public ServerSynchronizedRateLimitLease(bool isAcquired, int permits, TimeSpan? retryAfter)
    {
        IsAcquired = isAcquired;
        Permits = permits;
        RetryAfter = retryAfter;
    }

    public override bool IsAcquired { get; }

    public int Permits { get; }

    public TimeSpan? RetryAfter { get; }

    public override IEnumerable<string> MetadataNames =>
        RetryAfter.HasValue ? new[] { RetryAfterName } : Array.Empty<string>();

    public override bool TryGetMetadata(string metadataName, out object? metadata)
    {
        if (RetryAfter.HasValue && string.Equals(metadataName, RetryAfterName, StringComparison.Ordinal))
        {
            metadata = RetryAfter.Value;
            return true;
        }

        metadata = null;
        return false;
    }
}
