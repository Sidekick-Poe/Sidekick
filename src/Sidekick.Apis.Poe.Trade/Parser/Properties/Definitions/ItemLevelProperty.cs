using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ItemLevelProperty(IGameLanguageProvider gameLanguageProvider, GameType game) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionItemLevel.ToRegexIntCapture();

    public override List<Category> ValidCategories { get; } = [Category.Armour, Category.Weapon, Category.Flask, Category.Jewel, Category.Accessory, Category.Map, Category.Contract, Category.Sanctum, Category.Logbook];

    public override void Parse(Item item)
    {
        item.Properties.ItemLevel = GetInt(Pattern, item.Text);
    }

    public override Task<PropertyFilter?> GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.ItemLevel <= 0) return Task.FromResult<PropertyFilter?>(null);

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionItemLevel,
            NormalizeEnabled = false,
            NormalizeValue = normalizeValue,
            Value = item.Properties.ItemLevel,
            Checked = game == GameType.PathOfExile && item.Properties.ItemLevel >= 80 && item.Properties.MapTier == 0 && item.Header.Rarity != Rarity.Unique,
        };
        filter.ChangeFilterType(filterType);
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        switch (game)
        {
            case GameType.PathOfExile: query.Filters.GetOrCreateMiscFilters().Filters.ItemLevel = new StatFilterValue(intFilter); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateTypeFilters().Filters.ItemLevel = new StatFilterValue(intFilter); break;
        }
    }
}
