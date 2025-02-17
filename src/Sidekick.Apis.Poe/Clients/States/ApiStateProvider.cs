namespace Sidekick.Apis.Poe.Clients.States;

public class ApiStateProvider : IApiStateProvider
{
    public event Action? OnChange;

    private Dictionary<string, ApiState> States { get; set; } = new();

    public ApiState Get(string clientName)
    {
        if (States.TryGetValue(clientName, out var currentState))
        {
            return currentState;
        }

        return ApiState.Unknown;
    }

    public void Update(string clientName, ApiState state)
    {
        if (States.TryGetValue(clientName, out var currentState) && state == currentState)
        {
            return;
        }

        States.Remove(clientName);
        States.Add(clientName, state);
        OnChange?.Invoke();
    }
}
