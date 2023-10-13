using System.Diagnostics;
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
        private static TimeLimiter? timeConstraint = null;

        public PoeApiHandler(
            ILogger<PoeApiHandler> logger,
            IAuthenticationService authenticationService)
        {
            this.logger = logger;
            this.authenticationService = authenticationService;
            if (timeConstraint == null)
            {
                timeConstraint = TimeLimiter.Compose(
                    new CountByIntervalAwaitableConstraint(10, TimeSpan.FromSeconds(15)),
                    new CountByIntervalAwaitableConstraint(30, TimeSpan.FromSeconds(300))
                );
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await timeConstraint;

            var token = await authenticationService.GetAccessToken();

            if (String.IsNullOrEmpty(token))
            {
               return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response =  await base.SendAsync(request, cancellationToken);

            var rateDef = response.Headers.GetValues("x-rate-limit-account").FirstOrDefault();
            var rateState = response.Headers.GetValues("x-rate-limit-account-state").FirstOrDefault();

            logger.LogInformation($"[PoeApiHandler] Rate Defi.: {rateDef}");
            logger.LogInformation($"[PoeApiHandler] Rate State: {rateState}");
            return response;

        }
    }
}
