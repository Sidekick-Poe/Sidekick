using System.Net.Http.Headers;
using ComposableAsync;
using Microsoft.Extensions.Logging;
using RateLimiter;
using Sidekick.Apis.Poe.Authentication;

namespace Sidekick.Apis.Poe.Clients
{
    public class PoeApiHandler : HttpClientHandler
    {
        private readonly ILogger<PoeApiHandler> logger;
        private readonly IAuthenticationService authenticationService;
        private TimeLimiter timeConstraint;

        public PoeApiHandler(
            ILogger<PoeApiHandler> logger,
            IAuthenticationService authenticationService)
        {
            this.logger = logger;
            this.authenticationService = authenticationService;
            timeConstraint = TimeLimiter.Compose(
                new CountByIntervalAwaitableConstraint(15, TimeSpan.FromSeconds(10)),
                new CountByIntervalAwaitableConstraint(30, TimeSpan.FromSeconds(300))
            );
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await timeConstraint;

            var token = await authenticationService.GetAccessToken();

            if(String.IsNullOrEmpty(token))
            {
               return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);

        }
    }
}