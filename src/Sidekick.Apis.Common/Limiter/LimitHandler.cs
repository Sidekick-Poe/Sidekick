using System.Threading.RateLimiting;
using Microsoft.Extensions.Logging;

namespace Sidekick.Apis.Common.Limiter;

public class LimitHandler(ILogger logger)
{
    public event Action? OnChange;

    private ConcurrencyLimiter ConcurrencyLimiter { get; } = new(new ConcurrencyLimiterOptions()
    {
        PermitLimit = 1,
        QueueLimit = int.MaxValue,
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
    });

    public List<LimitRule> Rules { get; } = [];

    public async ValueTask<LimitLease> Lease(CancellationToken cancellationToken = default)
    {
        var leases = new List<RateLimitLease>();
        foreach (var limitRule in Rules)
        {
            logger.LogDebug($"Acquiring limit for {limitRule}");
            leases.Add(await limitRule.Limiter.AcquireAsync(cancellationToken: cancellationToken));
            logger.LogDebug($"Acquired limit for {limitRule}");
        }

        return new LimitLease()
        {
            Leases = leases,
            ConcurrencyLease = await ConcurrencyLimiter.AcquireAsync(cancellationToken: cancellationToken),
        };
    }

    public void HandleResponse(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        var headerRules = GetRulesFromHeader().ToList();
        foreach (var headerRule in headerRules)
        {
            var rule = Rules.FirstOrDefault(r => r.Name == headerRule.Name
                                                 && r.Policy == headerRule.Policy
                                                 && r.TimePeriod == headerRule.TimePeriod
                                                 && r.MaxHitCount == headerRule.MaxHitCount);
            if (rule == null)
            {
                rule = new LimitRule(headerRule.Policy, headerRule.Name, headerRule.MaxHitCount, headerRule.TimePeriod);
                Rules.Add(rule);
                // Bubble limiter changes (like timer-based replenishment) to UI listeners
                rule.Limiter.OnChange += () => OnChange?.Invoke();
            }

            rule.Limiter.UpdateFromServer(headerRule.CurrentHitCount, DateTimeOffset.UtcNow);
        }

        foreach (var rule in Rules)
        {
            var headerRule = headerRules.FirstOrDefault(r => r.Name == rule.Name
                                                             && r.Policy == rule.Policy
                                                             && r.TimePeriod == rule.TimePeriod
                                                             && r.MaxHitCount == rule.MaxHitCount);
            if (headerRule == null)
            {
                rule.Limiter.UpdateFromServer(rule.CurrentHitCount - 1, DateTimeOffset.UtcNow);
            }
        }

        // Notify listeners that snapshot may have changed due to new response headers
        OnChange?.Invoke();

        return;

        IEnumerable<HeaderRule> GetRulesFromHeader()
        {
            // Determine policy for this response. If absent, group under a default policy key.
            var policy = response.Headers.TryGetValues("X-Rate-Limit-Policy", out var policyHeader)
                ? policyHeader.FirstOrDefault() ?? "default"
                : "default";

            if (response.Headers.TryGetValues("X-Rate-Limit-Rules", out var headerRuleNames))
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
                    for (var i = 0; i < definitions.Length; i++)
                    {
                        var definitionParts = definitions[i].Split(':');
                        var stateParts = states[i].Split(':');
                        if (definitionParts.Length != 3
                            || !int.TryParse(definitionParts[0], out var maxHitCount)
                            || !int.TryParse(definitionParts[1], out var timePeriod)
                            || stateParts.Length != 3
                            || !int.TryParse(stateParts[0], out var currentHitCount))
                        {
                            continue;
                        }

                        yield return new HeaderRule(policy, ruleName, currentHitCount, maxHitCount, timePeriod);
                    }
                }
            }
        }
    }
}
