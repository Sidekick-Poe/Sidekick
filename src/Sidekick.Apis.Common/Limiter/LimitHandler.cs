using System.Threading.RateLimiting;

namespace Sidekick.Apis.Common.Limiter;

public class LimitHandler
{
    public event Action? OnChange;

    private ConcurrencyLimiter ConcurrencyLimiter { get; } = new(new ConcurrencyLimiterOptions()
    {
        PermitLimit = 1,
        QueueLimit = int.MaxValue,
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
    });

    // Maintain rules per policy so we don't lose state when switching endpoints with different policies
    private Dictionary<string, List<LimitRule>> RulesByPolicy { get; } = new();

    public IEnumerable<LimitRule> GetSnapshot()
    {
        // Return a simple, read-only snapshot of the current rate limit state per rule
        // Name: rule group name provided by server (e.g., account, trading)
        // MaxHitCount: maximum requests per period
        // TimePeriod: window length in seconds
        // Available: currently available tokens according to the TokenBucket limiter
        // Used: computed as Max - Available within the current window
        foreach (var rules in RulesByPolicy.Values)
        {
            foreach (var rule in rules)
            {
                yield return rule;
            }
        }
    }

    public async ValueTask<LimitLease> Lease(CancellationToken cancellationToken = default)
    {
        var leases = new List<RateLimitLease>();
        foreach (var kvp in RulesByPolicy)
        {
            foreach (var limitRule in kvp.Value)
            {
                leases.Add(await limitRule.Limiter.AcquireAsync(cancellationToken: cancellationToken));
            }
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

        // Determine policy for this response. If absent, group under a default policy key.
        var policy = response.Headers.TryGetValues("X-Rate-Limit-Policy", out var policyHeader)
            ? policyHeader.FirstOrDefault() ?? "default"
            : "default";

        if (response.Headers.TryGetValues("X-Rate-Limit-Rules", out var headerRuleNames))
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

                    headerRules.Add(new HeaderRule(policy, ruleName, currentHitCount, maxHitCount, timePeriod));
                }
            }
        }

        // Ensure a bucket exists for this policy
        if (!RulesByPolicy.TryGetValue(policy, out var rulesForPolicy))
        {
            rulesForPolicy = new List<LimitRule>();
            RulesByPolicy[policy] = rulesForPolicy;
        }

        // Remove rules that are no longer present for this policy
        rulesForPolicy.RemoveAll(limitRule => !headerRules.Any(headerRule => headerRule.Policy == policy && headerRule.MaxHitCount == limitRule.MaxHitCount && headerRule.Name == limitRule.Name && headerRule.TimePeriod == limitRule.TimePeriod));

        // Add or update rules for this policy
        foreach (var headerRule in headerRules)
        {
            if (headerRule.Policy != policy) continue;

            var limitRule = rulesForPolicy.FirstOrDefault(limitRule => headerRule.MaxHitCount == limitRule.MaxHitCount && headerRule.Name == limitRule.Name && headerRule.TimePeriod == limitRule.TimePeriod);
            if (limitRule == null)
            {
                limitRule = new LimitRule(policy, headerRule.Name, headerRule.MaxHitCount, headerRule.TimePeriod, () => OnChange?.Invoke());
                await limitRule.Limiter.AcquireAsync(headerRule.CurrentHitCount, cancellationToken);
                rulesForPolicy.Add(limitRule);
            }

            await limitRule.HandleResponse(headerRule.CurrentHitCount);
        }

        // Notify listeners that snapshot may have changed due to new response headers
        OnChange?.Invoke();
    }
}
