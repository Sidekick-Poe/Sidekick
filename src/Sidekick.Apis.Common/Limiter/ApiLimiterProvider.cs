using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
namespace Sidekick.Apis.Common.Limiter;

public class ApiLimiterProvider(ILogger<ApiLimiterProvider> logger)
{
    // Requested at the same time by the API handlers and the UI, so the dictionary must be thread-safe.
    private ConcurrentDictionary<string, Lazy<LimitHandler>> Handlers { get; } = new();

    public LimitHandler Get(string clientName)
    {
        // Lazy makes sure that a single handler is created per client, even when two threads ask at the same time.
        return Handlers.GetOrAdd(clientName, _ => new Lazy<LimitHandler>(() => new LimitHandler(logger))).Value;
    }
}
