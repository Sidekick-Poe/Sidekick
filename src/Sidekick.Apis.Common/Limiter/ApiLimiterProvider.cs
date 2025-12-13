using Microsoft.Extensions.Logging;
namespace Sidekick.Apis.Common.Limiter;

public class ApiLimiterProvider(ILogger<ApiLimiterProvider> logger)
{
    private Dictionary<string, LimitHandler> Handlers { get; } = [];

    public LimitHandler Get(string clientName)
    {
        if (Handlers.TryGetValue(clientName, out var currentState))
        {
            return currentState;
        }

        Handlers.Add(clientName, new LimitHandler(logger));
        return Handlers[clientName];
    }
}
