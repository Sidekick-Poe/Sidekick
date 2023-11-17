namespace Sidekick.Apis.Poe.Authentication
{
    public interface IAuthenticationService
    {
        event Action? OnAuthenticated;

        event Action? OnStateChanged;

        AuthenticationState CurrentState { get; }

        string? GetToken();

        Task Authenticate(bool reauthenticate = false);
    }
}
