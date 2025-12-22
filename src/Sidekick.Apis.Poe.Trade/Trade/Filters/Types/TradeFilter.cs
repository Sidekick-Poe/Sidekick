using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Types;

public class TradeFilter
{
    public bool ShowRow { get; init; } = true;

    public virtual bool Checked { get; set; }

    public string Text { get; set; } = string.Empty;

    public string? Hint { get; init; }

    public LineContentType Type { get; init; } = LineContentType.Simple;

    public virtual void PrepareTradeRequest(Query query, Item item) {}

    public required AutoSelectPreferences AutoSelect { get; init; }

    public async Task<bool> ShouldCheck(Item item)
    {
        var rules = await GetRules();
        if (rules.Count == 0) return false;

        foreach (var rule in rules)
        {
            if (RuleMatches(rule, item, filter)) return true;
        }

        return false;
    }
}