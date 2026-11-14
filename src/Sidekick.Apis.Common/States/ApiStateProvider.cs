using System.Collections.Concurrent;

namespace Sidekick.Apis.Common.States;

public class ApiStateProvider : IApiStateProvider
{
    public event Action? OnChange;

    // Updated from concurrent API requests, so the dictionary must be thread-safe.
    private ConcurrentDictionary<string, ApiState> States { get; } = new();

    public ApiState Get(string clientName)
    {
        return States.TryGetValue(clientName, out var currentState) ? currentState : ApiState.Unknown;
    }

    public void Update(string clientName, ApiState state)
    {
        var previousState = ApiState.Unknown;
        var hadState = false;
        States.AddOrUpdate(
            clientName,
            state,
            (_, currentState) =>
            {
                hadState = true;
                previousState = currentState;
                return state;
            });

        if (hadState && previousState == state) return;

        OnChange?.Invoke();
    }
}
