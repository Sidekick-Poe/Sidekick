using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Models;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Trade;

public class TradeFilterService : ITradeFilterService
{
    private readonly IPropertyParser propertyParser;
    private readonly ISettingsService settingsService;
    public int Priority => 0;

    private bool? EnableAllFilters;
    private bool EnableFiltersByRegexIsValid;
    private Regex? EnableFiltersByRegex;

    public TradeFilterService(IPropertyParser propertyParser, ISettingsService settingsService)
    {
        this.propertyParser = propertyParser;
        this.settingsService = settingsService;

        settingsService.OnSettingsChanged += OnSettingsChanged;
    }

    public async Task Initialize()
    {
        await GetFiltersSettings();
    }

    private void OnSettingsChanged(string[] keys)
    {
        if (!keys.Any(x => x == SettingKeys.PriceCheckEnableAllFilters || x == SettingKeys.PriceCheckEnableFiltersByRegex))
        {
            return;
        }

        _ = Task.Run(GetFiltersSettings);
    }

    private async Task GetFiltersSettings()
    {
        EnableAllFilters = await settingsService.GetBool(SettingKeys.PriceCheckEnableAllFilters);

        var enableFiltersByRegexSetting = await settingsService.GetString(SettingKeys.PriceCheckEnableFiltersByRegex);
        if (!string.IsNullOrWhiteSpace(enableFiltersByRegexSetting))
        {
            EnableFiltersByRegex = new Regex(enableFiltersByRegexSetting, RegexOptions.IgnoreCase);
            EnableFiltersByRegexIsValid = true;
        }
        else
        {
            EnableFiltersByRegexIsValid = false;
        }
    }

    private bool ShouldFilterBeEnabled(string modifierLineText)
    {
        if (EnableAllFilters == true)
        {
            return true;
        }
        else if (EnableFiltersByRegexIsValid)
        {
            return EnableFiltersByRegex?.IsMatch(modifierLineText) == true;
        }

        return false;
    }

    public IEnumerable<ModifierFilter> GetModifierFilters(Item item)
    {
        // No filters for divination cards, etc.
        if (item.Header.Category == Category.DivinationCard || item.Header.Category == Category.Gem || item.Header.Category == Category.ItemisedMonster || item.Header.Category == Category.Leaguestone || item.Header.Category == Category.Unknown)
        {
            yield break;
        }

        foreach (var modifierLine in item.ModifierLines)
        {
            var modifier = new ModifierFilter(modifierLine);
            modifier.Checked = ShouldFilterBeEnabled(modifierLine.Text);
            yield return modifier;
        }
    }

    public IEnumerable<PseudoModifierFilter> GetPseudoModifierFilters(Item item)
    {
        // No filters for divination cards, etc.
        if (item.Header.Category == Category.DivinationCard || item.Header.Category == Category.Gem || item.Header.Category == Category.ItemisedMonster || item.Header.Category == Category.Leaguestone || item.Header.Category == Category.Unknown || item.Header.Category == Category.Currency)
        {
            yield break;
        }

        foreach (var modifier in item.PseudoModifiers)
        {
            yield return new PseudoModifierFilter()
            {
                PseudoModifier = modifier,
                Checked = false,
            };
        }
    }

    public async Task<PropertyFilters> GetPropertyFilters(Item item)
    {
        var filters = await propertyParser.GetFilters(item);
        var result = new PropertyFilters()
        {
            Filters = filters,
        };

        var preferItemClass = await settingsService.GetEnum<DefaultItemClassFilter>(SettingKeys.PriceCheckItemClassFilter) ?? DefaultItemClassFilter.BaseType;
        if (preferItemClass == DefaultItemClassFilter.ItemClass)
        {
            result.ClassFilterApplied = true;
            result.BaseTypeFilterApplied = false;
        }
        else
        {
            result.BaseTypeFilterApplied = true;
            result.ClassFilterApplied = false;
        }

        return result;
    }
}
