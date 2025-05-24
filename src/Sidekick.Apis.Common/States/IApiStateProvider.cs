namespace Sidekick.Apis.Common.States;

public interface IApiStateProvider
{
    event Action? OnChange;

    ApiState Get(string clientName);
    void Update(string clientName, ApiState state);
}
