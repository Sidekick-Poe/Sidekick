namespace Sidekick.Apis.Poe.Authentication
{
    public interface IAuthenticationService
    {
        event Action? OnAuthenticated;

        event Action? OnStateChanged;

        Task<AuthenticationState> GetCurrentState();

        Task<string?> GetToken();

        Task Authenticate(bool reauthenticate = false);
    }
}
