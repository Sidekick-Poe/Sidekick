using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ItemLevelProperty(IGameLanguageProvider gameLanguageProvider, GameType game) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionItemLevel.ToRegexIntCapture();

    public override List<Category> ValidCategories { get; } = [Category.Armour, Category.Weapon, Category.Flask, Category.Jewel, Category.Accessory, Category.Map, Category.Contract, Category.Sanctum, Category.Logbook];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem)
    {
        itemProperties.ItemLevel = GetInt(Pattern, parsingItem);
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.ItemLevel <= 0) return null;

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionItemLevel,
            NormalizeEnabled = false,
            NormalizeValue = normalizeValue,
            Value = item.Properties.ItemLevel,
            Checked = game == GameType.PathOfExile && item.Properties.ItemLevel >= 80 && item.Properties.MapTier == 0 && item.Header.Rarity != Rarity.Unique,
        };
        filter.ChangeFilterType(filterType);
        return filter;
    }

    public override void PrepareTradeRequest(SearchFilters searchFilters, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        switch (game)
        {
            case GameType.PathOfExile: searchFilters.GetOrCreateMiscFilters().Filters.ItemLevel = new StatFilterValue(intFilter); break;

            case GameType.PathOfExile2: searchFilters.GetOrCreateTypeFilters().Filters.ItemLevel = new StatFilterValue(intFilter); break;
        }
    }
}
