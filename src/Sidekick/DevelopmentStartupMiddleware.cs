using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Sidekick
{
    public class DevelopmentStartupMiddleware
    {
        private static bool HasStartup { get; set; } = false;
        private static readonly Regex IgnorePaths = new("^\\/_blazor.*$");

        private readonly RequestDelegate next;

        public DevelopmentStartupMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task InvokeAsync(HttpContext httpContext)
        {
            if (!HasStartup && !IgnorePaths.IsMatch(httpContext.Request.Path.Value))
            {
                HasStartup = true;

                if (httpContext.Request.Path.Value != "/update")
                {
                    httpContext.Response.Redirect("/update");
                    return httpContext.Response.CompleteAsync();
                }
            }

            return next(httpContext);
        }
    }
}
