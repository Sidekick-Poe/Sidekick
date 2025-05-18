namespace Sidekick.Apis.Poe.Clients.Limiter;

public class ApiLimiterProvider
{
    private Dictionary<string, LimitHandler> Handlers { get; } = [];

    internal LimitHandler Get(string clientName)
    {
        if (Handlers.TryGetValue(clientName, out var currentState))
        {
            return currentState;
        }

        Handlers.Add(clientName, new LimitHandler());
        return Handlers[clientName];
    }
}
