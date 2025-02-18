using Sidekick.Apis.Poe.Parser.Properties;
using Sidekick.Apis.Poe.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade;

public class TradeFilterService
(
    IPropertyParser propertyParser,
    ISettingsService settingsService
) : ITradeFilterService
{
    public IEnumerable<ModifierFilter> GetModifierFilters(Item item)
    {
        // No filters for divination cards, etc.
        if (item.Header.Category == Category.DivinationCard || item.Header.Category == Category.Gem || item.Header.Category == Category.ItemisedMonster || item.Header.Category == Category.Leaguestone || item.Header.Category == Category.Unknown)
        {
            yield break;
        }

        foreach (var modifierLine in item.ModifierLines)
        {
            yield return new ModifierFilter(modifierLine);
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
