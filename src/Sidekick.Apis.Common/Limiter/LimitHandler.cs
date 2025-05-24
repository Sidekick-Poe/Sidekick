using System.Threading.RateLimiting;

namespace Sidekick.Apis.Common.Limiter;

public class LimitHandler
{
    private ConcurrencyLimiter ConcurrencyLimiter { get; } = new(new ConcurrencyLimiterOptions()
    {
        PermitLimit = 1,
        QueueLimit = int.MaxValue,
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
    });

    private List<LimitRule> Rules { get; } = [];

    public async ValueTask<LimitLease> Lease(CancellationToken cancellationToken = default)
    {
        var leases = new List<RateLimitLease>();
        foreach (var limitRule in Rules)
        {
            leases.Add(await limitRule.Limiter.AcquireAsync(cancellationToken: cancellationToken));
        }

        return new LimitLease()
        {
            Leases = leases,
            ConcurrencyLease = await ConcurrencyLimiter.AcquireAsync(cancellationToken: cancellationToken),
        };
    }

    public async Task HandleResponse(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        var headerRules = new List<HeaderRule>();

        if (response.Headers.TryGetValues("X-Rate-Limit-Policy", out var _) && response.Headers.TryGetValues("X-Rate-Limit-Rules", out var headerRuleNames))
        {
            var ruleNames = headerRuleNames.First().Split(',');
            foreach (var ruleName in ruleNames)
            {
                if (!response.Headers.TryGetValues($"X-Rate-Limit-{ruleName}", out var headerRule) || !response.Headers.TryGetValues($"X-Rate-Limit-{ruleName}-State", out var headerRuleState))
                {
                    continue;
                }

                var definitions = headerRule.First().Split(',');
                var states = headerRuleState.First().Split(',');
                for (var i = 0; i < definitions.Count(); i++)
                {
                    var definitionParts = definitions[i].Split(':');
                    var stateParts = states[i].Split(':');

                    if (definitionParts.Length != 3 || !int.TryParse(definitionParts[0], out var maxHitCount) || !int.TryParse(definitionParts[1], out var timePeriod) || stateParts.Length != 3 || !int.TryParse(stateParts[0], out var currentHitCount))
                    {
                        continue;
                    }

                    headerRules.Add(new HeaderRule(ruleName, currentHitCount, maxHitCount, timePeriod));
                }
            }
        }

        Rules.RemoveAll(limitRule => !headerRules.Any(headerRule => headerRule.MaxHitCount == limitRule.MaxHitCount && headerRule.Name == limitRule.Name && headerRule.TimePeriod == limitRule.TimePeriod));
        foreach (var headerRule in headerRules)
        {
            var limitRule = Rules.FirstOrDefault(limitRule => headerRule.MaxHitCount == limitRule.MaxHitCount && headerRule.Name == limitRule.Name && headerRule.TimePeriod == limitRule.TimePeriod);
            if (limitRule == null)
            {
                limitRule = new LimitRule(headerRule.Name, headerRule.MaxHitCount, headerRule.TimePeriod);
                await limitRule.Limiter.AcquireAsync(headerRule.CurrentHitCount, cancellationToken);
                Rules.Add(limitRule);
            }

            await limitRule.HandleResponse(headerRule.CurrentHitCount);
        }
    }
}
