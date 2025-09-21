using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ItemClassProperty(
    IGameLanguageProvider gameLanguageProvider,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    public override List<Category> ValidCategories { get; } = [];

    private IFilterProvider FilterProvider => serviceProvider.GetRequiredService<IFilterProvider>();

    private ISettingsService SettingsService => serviceProvider.GetRequiredService<ISettingsService>();

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem, ItemHeader header) {}

    public override async Task<PropertyFilter?> GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Header.Rarity is not (Rarity.Rare or Rarity.Magic or Rarity.Normal)) return null;
        if (item.Header.ItemClass == ItemClass.Unknown) return null;

        var classLabel = FilterProvider.TypeCategory?.Option.Options.FirstOrDefault(x => x.Id == item.Header.ItemClass.GetValueAttribute())?.Text;
        if (classLabel == null || item.Header.ApiType == null) return null;

        var preferItemClass = await SettingsService.GetEnum<DefaultItemClassFilter>(SettingKeys.PriceCheckItemClassFilter) ?? DefaultItemClassFilter.BaseType;

        var filter = new ItemClassPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionRarity,
            ItemClass = classLabel,
            BaseType = item.Header.ApiType,
            Checked = preferItemClass == DefaultItemClassFilter.ItemClass,
        };
        return filter;
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked || filter is not ItemClassPropertyFilter) return;

        query.Type = null;
        query.Filters.GetOrCreateTypeFilters().Filters.Category = GetCategoryFilter(item.Header.ItemClass);
    }

    private static SearchFilterOption? GetCategoryFilter(ItemClass itemClass)
    {
        var enumValue = itemClass.GetValueAttribute();
        if (string.IsNullOrEmpty(enumValue)) return null;

        return new SearchFilterOption(enumValue);
    }
}
