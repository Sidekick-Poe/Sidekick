using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RequiresLevelProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionLevel.ToRegexIntCapture();

    private Regex RequiresPattern { get; } = new($@"^{gameLanguageProvider.Language.DescriptionRequires}.*{gameLanguageProvider.Language.DescriptionLevel}\s*(\d+)");

    public override List<Category> ValidCategories { get; } = [Category.Armour, Category.Weapon, Category.Accessory, Category.Flask, Category.Graft];

    public override void Parse(Item item)
    {
        foreach (var block in item.Text.Blocks)
        {
            item.Properties.RequiresLevel = GetInt(Pattern, block);
            if (item.Properties.RequiresLevel == 0) item.Properties.RequiresLevel = GetInt(RequiresPattern, block);
            if (item.Properties.RequiresLevel == 0) continue;

            block.Parsed = true;
            return;
        }
    }

    public override Task<PropertyFilter?> GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.RequiresLevel <= 0) return Task.FromResult<PropertyFilter?>(null);

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionRequiresLevel,
            NormalizeEnabled = false,
            NormalizeValue = normalizeValue,
            Value = item.Properties.RequiresLevel,
        };
        filter.ChangeFilterType(filterType);
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        query.Filters.GetOrCreateRequirementsFilters().Filters.Level = new StatFilterValue(intFilter);
    }
}
