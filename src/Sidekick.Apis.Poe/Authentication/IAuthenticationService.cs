using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Authentication
{
    public interface IAuthenticationService
    {
        Task Authenticate();

        Task<string> AuthenticationCallback(string auth, string state);

        string GetAccessToken();

        bool IsAuthenticated();

        bool IsAuthenticating();
    }
}
