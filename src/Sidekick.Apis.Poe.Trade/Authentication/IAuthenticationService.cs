namespace Sidekick.Apis.Poe.Trade.Authentication;

public interface IAuthenticationService
{
    event Action? OnStateChanged;

    Task<AuthenticationState> GetCurrentState();

    Task<DateTimeOffset?> GetExpiration();

    Task InitializeHttpRequest(HttpRequestMessage request, CancellationToken cancellationToken = default);

    Task Authenticate(bool reauthenticate = false, CancellationToken cancellationToken = default);
}
