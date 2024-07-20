using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Authentication
{
    public interface IAuthenticationService : IInitializableService
    {
        event Action? OnStateChanged;

        Task<AuthenticationState> GetCurrentState();

        Task<string?> GetToken();

        Task Authenticate(bool reauthenticate = false);
    }
}
