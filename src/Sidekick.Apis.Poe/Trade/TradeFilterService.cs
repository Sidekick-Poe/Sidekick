using Sidekick.Apis.Poe.Parser.Properties;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade;

public class TradeFilterService : ITradeFilterService
{
    private readonly IPropertyParser propertyParser;
    private readonly ISettingsService settingsService;

    public TradeFilterService(IPropertyParser propertyParser, ISettingsService settingsService)
    {
        this.propertyParser = propertyParser;
        this.settingsService = settingsService;
    }

    public async Task<List<ModifierFilter>> GetModifierFilters(Item item)
    {
        // No filters for divination cards, etc.
        if (item.Header.Category == Category.DivinationCard || item.Header.Category == Category.Gem || item.Header.Category == Category.ItemisedMonster || item.Header.Category == Category.Leaguestone || item.Header.Category == Category.Unknown)
        {
            return await Task.FromResult(new List<ModifierFilter>());
        }

        var enableAllFilters = await settingsService.GetBool(SettingKeys.PriceCheckEnableAllFilters);

        var filters = new List<ModifierFilter>();
        foreach (var modifierLine in item.ModifierLines)
        {
            var modifier = new ModifierFilter(modifierLine);
            modifier.Checked = enableAllFilters;
            filters.Add(modifier);
        }

        return await Task.FromResult(filters);
    }

    public async Task<List<PseudoModifierFilter>> GetPseudoModifierFilters(Item item)
    {
        // No filters for divination cards, etc.
        if (item.Header.Category == Category.DivinationCard || item.Header.Category == Category.Gem || item.Header.Category == Category.ItemisedMonster || item.Header.Category == Category.Leaguestone || item.Header.Category == Category.Unknown || item.Header.Category == Category.Currency)
        {
            return await Task.FromResult(new List<PseudoModifierFilter>());
        }

        var filters = new List<PseudoModifierFilter>();
        foreach (var modifier in item.PseudoModifiers)
        {
            filters.Add(new PseudoModifierFilter()
            {
                PseudoModifier = modifier,
                Checked = false,
            });
        }

        return await Task.FromResult(filters);
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
