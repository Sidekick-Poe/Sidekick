using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RequiresStrengthProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionRequiresStr.ToRegexIntCapture();

    private Regex RequiresPattern { get; } = new($@"^{gameLanguageProvider.Language.DescriptionRequires}.*?(\d+)\s*{gameLanguageProvider.Language.DescriptionRequiresStr}");

    public override List<Category> ValidCategories { get; } = [Category.Armour, Category.Weapon, Category.Accessory, Category.Flask, Category.Graft];

    public override void Parse(Item item)
    {
        foreach (var block in item.Text.Blocks)
        {
            item.Properties.RequiresStrength = GetInt(Pattern, block);
            if (item.Properties.RequiresStrength == 0) item.Properties.RequiresStrength = GetInt(RequiresPattern, block);
            if (item.Properties.RequiresStrength == 0) continue;

            block.Parsed = true;
            return;
        }
    }

    public override Task<PropertyFilter?> GetFilter(Item item)
    {
        if (item.Properties.RequiresStrength <= 0) return Task.FromResult<PropertyFilter?>(null);

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionRequiresStr,
            NormalizeEnabled = false,
            Value = item.Properties.RequiresStrength,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        query.Filters.GetOrCreateRequirementsFilters().Filters.Strength = new StatFilterValue(intFilter);
    }
}
