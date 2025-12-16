using System.Text.Json;
using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.AutoSelect.Models;
using Sidekick.Apis.Poe.Trade.Parser.Filters;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.AutoSelect;

public class FilterAutoSelectService(ISettingsService settingsService) : IFilterAutoSelectService
{
    private List<AutoSelectRule>? cachedRules;

    public async Task<bool> ShouldCheck(Item item, IAutoSelectableFilter filter)
    {
        var rules = await GetRules();
        if (rules.Count == 0) return false;

        foreach (var rule in rules)
        {
            if (RuleMatches(rule, item, filter)) return true;
        }

        return false;
    }

    private async Task<List<AutoSelectRule>> GetRules()
    {
        if (cachedRules != null) return cachedRules;

        var json = await settingsService.GetString(SettingKeys.PriceCheckAutoSelectRules);
        if (string.IsNullOrWhiteSpace(json)) return cachedRules = [];

        try
        {
            var parsed = JsonSerializer.Deserialize<List<AutoSelectRule>>(json) ?? [];
            return cachedRules = parsed;
        }
        catch
        {
            // Invalid JSON; ignore rules
            return cachedRules = [];
        }
    }

    private static bool RuleMatches(AutoSelectRule rule, Item item, IAutoSelectableFilter filter)
    {
        foreach (var condition in rule.All)
        {
            if (!ConditionMatches(condition, item, filter)) return false;
        }

        return true;
    }

    private static bool ConditionMatches(AutoSelectCondition condition, Item item, IAutoSelectableFilter filter)
    {
        switch (condition.Type)
        {
            case AutoSelectConditionType.ItemRarityIs:
                if (string.IsNullOrWhiteSpace(condition.Value)) return false;
                return string.Equals(item.Properties.Rarity.ToString(), condition.Value, StringComparison.OrdinalIgnoreCase)
                       || string.Equals(item.Properties.ItemRarity.ToString(), condition.Value, StringComparison.OrdinalIgnoreCase);

            case AutoSelectConditionType.ItemClassIs:
                if (string.IsNullOrWhiteSpace(condition.Value)) return false;
                return string.Equals(item.Properties.ItemClass.ToString(), condition.Value, StringComparison.OrdinalIgnoreCase);

            case AutoSelectConditionType.FilterTextRegex:
                if (string.IsNullOrWhiteSpace(condition.Value)) return false;
                try
                {
                    return Regex.IsMatch(filter.Text, condition.Value, RegexOptions.IgnoreCase);
                }
                catch
                {
                    // Invalid regex, ignore
                    return false;
                }

            default:
                return false;
        }
    }
}
