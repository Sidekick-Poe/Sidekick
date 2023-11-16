namespace Sidekick.Apis.Poe.Authentication
{
    public interface IAuthenticationService
    {
        event Action? OnStateChanged;

        AuthenticationState CurrentState { get; }

        string? GetToken();

        Task Authenticate();
    }
}
