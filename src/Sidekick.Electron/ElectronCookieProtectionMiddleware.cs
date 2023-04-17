using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Sidekick.Electron
{
    /// <summary>
    /// Middleware that checks if the requests being made are from the Electron app and nothing else.
    /// </summary>
    public class ElectronCookieProtectionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ElectronCookieProtection electronAuthorizationCookie;

        public ElectronCookieProtectionMiddleware(RequestDelegate next, ElectronCookieProtection electronAuthorizationCookie)
        {
            this.next = next;
            this.electronAuthorizationCookie = electronAuthorizationCookie;
        }

        public Task Invoke(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue(electronAuthorizationCookie.Name, out var cookie) && cookie == electronAuthorizationCookie.Value)
            {
                return next.Invoke(context);
            }

            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }
    }
}
