using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class GemLevelProperty
(
    IGameLanguageProvider gameLanguageProvider
) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionLevel.ToRegexIntCapture();

    private Regex IntCapture { get; } = new("(\\d+)");

    public override List<Category> ValidCategories { get; } = [Category.Gem];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.GemLevel = GetInt(Pattern, propertyBlock);

        if (item.Header.ItemClass is ItemClass.UncutSkillGem or ItemClass.UncutSupportGem or ItemClass.UncutSpiritGem)
        {
            item.Properties.GemLevel = GetInt(IntCapture, item.Text.Blocks[0]);
        }

        if (item.Properties.GemLevel > 0) propertyBlock.Parsed = true;
    }

    public override Task<PropertyFilter?> GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.GemLevel <= 0) return Task.FromResult<PropertyFilter?>(null);

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionLevel,
            NormalizeEnabled = false,
            NormalizeValue = normalizeValue,
            Value = item.Properties.GemLevel,
            Checked = true,
        };
        filter.ChangeFilterType(filterType);
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        query.Filters.GetOrCreateMiscFilters().Filters.GemLevel = new StatFilterValue(intFilter);
    }
}
