using System.Net.Http.Headers;
using System.Threading.RateLimiting;
using Sidekick.Apis.Poe.Authentication;
using Sidekick.Apis.Poe.Clients.Limiter;
using Sidekick.Apis.Poe.Clients.States;

namespace Sidekick.Apis.Poe.Clients
{
    public class PoeApiHandler : HttpClientHandler
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IApiStateProvider apiStateProvider;

        private readonly ConcurrencyLimiter concurrencyLimiter;
        private List<LimitRule> limitRules = new();

        public PoeApiHandler(IAuthenticationService authenticationService, IApiStateProvider apiStateProvider)
        {
            this.authenticationService = authenticationService;
            this.apiStateProvider = apiStateProvider;
            concurrencyLimiter = new ConcurrencyLimiter(new ConcurrencyLimiterOptions()
            {
                PermitLimit = 1,
                QueueLimit = int.MaxValue,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            });
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            apiStateProvider.Update(ClientNames.POECLIENT, ApiState.Throttled);

            using var concurrencyLease = await concurrencyLimiter.AcquireAsync(cancellationToken: cancellationToken);

            var leases = new List<RateLimitLease>();
            foreach (var limitRule in limitRules)
            {
                leases.Add(await limitRule.Limiter.AcquireAsync(cancellationToken: cancellationToken));
            }

            apiStateProvider.Update(ClientNames.POECLIENT, ApiState.Working);

            var token = authenticationService.GetToken();
            if (string.IsNullOrEmpty(token))
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await base.SendAsync(request, cancellationToken);
            await ParseRateLimitHeaders(response);

            foreach (var lease in leases)
            {
                lease.Dispose();
            }

            return response;
        }

        private async Task ParseRateLimitHeaders(HttpResponseMessage response)
        {
            var headerRules = new List<HeaderRule>();

            if (response.Headers.TryGetValues("X-Rate-Limit-Policy", out var headerPolicies)
             && response.Headers.TryGetValues("X-Rate-Limit-Rules", out var headerRuleNames))
            {
                var ruleNames = headerRuleNames.First().Split(',');
                foreach (var ruleName in ruleNames)
                {
                    if (!response.Headers.TryGetValues($"X-Rate-Limit-{ruleName}", out var headerRule)
                     || !response.Headers.TryGetValues($"X-Rate-Limit-{ruleName}-State", out var headerRuleState))
                    {
                        continue;
                    }

                    var definitions = headerRule.First().Split(',');
                    var states = headerRuleState.First().Split(',');
                    for (var i = 0; i < definitions.Count(); i++)
                    {
                        var definitionParts = definitions[i].Split(':');
                        var stateParts = states[i].Split(':');

                        if (definitionParts.Count() != 3
                         || !int.TryParse(definitionParts[0], out var maxHitCount)
                         || !int.TryParse(definitionParts[1], out var timePeriod)
                         || stateParts.Count() != 3
                         || !int.TryParse(stateParts[0], out var currentHitCount))
                        {
                            continue;
                        }

                        headerRules.Add(new HeaderRule(ruleName, currentHitCount, maxHitCount, timePeriod));
                    }
                }
            }

            limitRules.RemoveAll(limitRule => !headerRules.Any(headerRule => headerRule.MaxHitCount == limitRule.MaxHitCount && headerRule.Name == limitRule.Name && headerRule.TimePeriod == limitRule.TimePeriod));
            foreach (var headerRule in headerRules)
            {
                var limitRule = limitRules.FirstOrDefault(limitRule => headerRule.MaxHitCount == limitRule.MaxHitCount && headerRule.Name == limitRule.Name && headerRule.TimePeriod == limitRule.TimePeriod);
                if (limitRule == null)
                {
                    limitRule = new LimitRule(headerRule.Name, headerRule.MaxHitCount, headerRule.TimePeriod);
                    await limitRule.Limiter.AcquireAsync(permitCount: headerRule.CurrentHitCount);
                    limitRules.Add(limitRule);
                }

                await limitRule.HandleResponse(headerRule.CurrentHitCount);
            }

            if (response.Headers.TryGetValues("Retry-After", out var retryAfter))
            {
                apiStateProvider.Update(ClientNames.POECLIENT, ApiState.TimedOut);
                await Task.Delay(TimeSpan.FromSeconds(int.Parse(retryAfter.First()) + 5));
                throw new TimeoutException();
            }
        }
    }
}
