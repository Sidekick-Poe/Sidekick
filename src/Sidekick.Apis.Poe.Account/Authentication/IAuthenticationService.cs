namespace Sidekick.Apis.Poe.Account.Authentication;

public interface IAuthenticationService
{
    event Action? OnStateChanged;

    Task<AuthenticationState> GetCurrentState();

    Task InitializeHttpRequest(HttpRequestMessage request);

    Task Authenticate(bool reauthenticate = false, CancellationToken cancellationToken = default);
}
