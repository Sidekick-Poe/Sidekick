using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Models;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class SpiritProperty
(
    IGameLanguageProvider gameLanguageProvider,
    GameType game
) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionSpirit.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionSpirit.ToRegexIsAugmented();

    public override List<Category> ValidCategories { get; } = [Category.Weapon, Category.Armour];

    public override void Parse(ItemProperties itemProperties, ParsingItem parsingItem, ItemHeader header)
    {
        if(game == GameType.PathOfExile) return;
        var propertyBlock = parsingItem.Blocks[1];
        itemProperties.Spirit = GetInt(Pattern, propertyBlock);
        if (itemProperties.Spirit == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) itemProperties.AugmentedProperties.Add(nameof(ItemProperties.Spirit));
    }

    public override BooleanPropertyFilter? GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (game == GameType.PathOfExile || item.Properties.Spirit <= 0) return null;

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionSpirit,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.Spirit,
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.Spirit)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        filter.ChangeFilterType(filterType);
        return filter;
    }

    public override void PrepareTradeRequest(Query query, Item item, BooleanPropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        switch (game)
        {
            case GameType.PathOfExile: break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.Spirit = new StatFilterValue(intFilter); break;
        }
    }
}
