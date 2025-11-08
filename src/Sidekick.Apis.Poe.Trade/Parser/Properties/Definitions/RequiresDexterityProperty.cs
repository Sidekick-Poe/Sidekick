using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RequiresDexterityProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionRequiresDex.ToRegexIntCapture();

    private Regex RequiresPattern { get; } = new($@"^{gameLanguageProvider.Language.DescriptionRequires}.*?(\d+)\s*{gameLanguageProvider.Language.DescriptionRequiresDex}");

    public override List<Category> ValidItemClasses { get; } = [Category.Armour, Category.Weapon, Category.Accessory, Category.Flask, Category.Graft];

    public override void Parse(Item item)
    {
        foreach (var block in item.Text.Blocks)
        {
            item.Properties.RequiresDexterity = GetInt(Pattern, block);
            if (item.Properties.RequiresDexterity == 0) item.Properties.RequiresDexterity = GetInt(RequiresPattern, block);
            if (item.Properties.RequiresDexterity == 0) continue;

            block.Parsed = true;
            return;
        }
    }

    public override Task<PropertyFilter?> GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.RequiresDexterity <= 0) return Task.FromResult<PropertyFilter?>(null);

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionRequiresDex,
            NormalizeEnabled = false,
            NormalizeValue = normalizeValue,
            Value = item.Properties.RequiresDexterity,
        };
        filter.ChangeFilterType(filterType);
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        query.Filters.GetOrCreateRequirementsFilters().Filters.Dexterity = new StatFilterValue(intFilter);
    }
}
