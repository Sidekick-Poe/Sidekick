using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Parser.Pseudo.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Trade;

public class TradeFilterService(ISettingsService settingsService) : ITradeFilterService
{
    private static bool ShouldFilterBeEnabled(bool enableAllFilters, string? enableFiltersByRegexSetting, string modifierLineText)
    {
        if (enableAllFilters) return true;

        if (string.IsNullOrWhiteSpace(enableFiltersByRegexSetting)) return false;

        var enableFiltersByRegex = new Regex(enableFiltersByRegexSetting, RegexOptions.IgnoreCase);
        return enableFiltersByRegex.IsMatch(modifierLineText);
    }

    public async Task<List<ModifierFilter>> GetModifierFilters(Item item)
    {
        // No filters for divination cards, etc.
        if (item.Header.Category == Category.DivinationCard || item.Header.Category == Category.Gem || item.Header.Category == Category.ItemisedMonster || item.Header.Category == Category.Leaguestone || item.Header.Category == Category.Unknown)
        {
            return [];
        }

        var enableAllFilters = await settingsService.GetBool(SettingKeys.PriceCheckEnableAllFilters);
        var enableFiltersByRegexSetting = await settingsService.GetString(SettingKeys.PriceCheckEnableFiltersByRegex);

        var result = new List<ModifierFilter>();
        foreach (var modifierLine in item.ModifierLines)
        {
            var modifier = new ModifierFilter(modifierLine)
            {
                Checked = ShouldFilterBeEnabled(enableAllFilters, enableFiltersByRegexSetting, modifierLine.Text),
            };
            result.Add(modifier);
        }

        return result;
    }

    public IEnumerable<PseudoFilter> GetPseudoModifierFilters(Item item)
    {
        // No filters for divination cards, etc.
        if (item.Header.Category == Category.DivinationCard || item.Header.Category == Category.Gem || item.Header.Category == Category.ItemisedMonster || item.Header.Category == Category.Leaguestone || item.Header.Category == Category.Unknown || item.Header.Category == Category.Currency)
        {
            yield break;
        }

        foreach (var modifier in item.PseudoModifiers)
        {
            yield return new PseudoFilter()
            {
                PseudoModifier = modifier,
                Checked = false,
            };
        }
    }
}
