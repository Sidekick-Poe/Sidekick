namespace Sidekick.Apis.Poe.Clients.States;

public interface IApiStateProvider
{
    event Action? OnChange;

    ApiState Get(string clientName);
    void Update(string clientName, ApiState state);
}
