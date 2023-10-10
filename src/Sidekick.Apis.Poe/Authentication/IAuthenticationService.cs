using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Authentication
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Opens an authentication page on the path of exile website if the user is not authenticated already.
        /// </summary>
        /// <returns>A task.</returns>
        Task Authenticate();


        Task AuthenticationCallback(string auth, string state);

        /// <summary>
        /// If we have the token already, return it.
        /// Starts the authentication process if the token is not set or is expired.
        /// </summary>
        /// <returns></returns>
        Task<string> GetAccessToken();
    }
}
