using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RequiresIntelligenceProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionRequiresInt.ToRegexIntCapture();

    private Regex RequiresPattern { get; } = new($@"^{gameLanguageProvider.Language.DescriptionRequires}.*?(\d+)\s*{gameLanguageProvider.Language.DescriptionRequiresInt}");

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Equipment,
        ..ItemClassConstants.Weapons,
        ItemClass.Graft,
    ];

    public override void Parse(Item item)
    {
        foreach (var block in item.Text.Blocks)
        {
            item.Properties.RequiresIntelligence = GetInt(Pattern, block);
            if (item.Properties.RequiresIntelligence == 0) item.Properties.RequiresIntelligence = GetInt(RequiresPattern, block);
            if (item.Properties.RequiresIntelligence == 0) continue;

            block.Parsed = true;
            return;
        }
    }

    public override Task<PropertyFilter?> GetFilter(Item item)
    {
        if (item.Properties.RequiresIntelligence <= 0) return Task.FromResult<PropertyFilter?>(null);

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionRequiresInt,
            NormalizeEnabled = false,
            Value = item.Properties.RequiresIntelligence,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        query.Filters.GetOrCreateRequirementsFilters().Filters.Intelligence = new StatFilterValue(intFilter);
    }
}
