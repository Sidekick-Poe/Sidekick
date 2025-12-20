using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RequiresLevelProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionLevel.ToRegexIntCapture();

    private Regex RequiresPattern { get; } = new($@"^{gameLanguageProvider.Language.DescriptionRequires}.*{gameLanguageProvider.Language.DescriptionLevel}\s*(\d+)");

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Equipment,
        ..ItemClassConstants.Weapons,
        ..ItemClassConstants.Accessories,
        ..ItemClassConstants.Flasks,
        ItemClass.Graft,
    ];

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

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.RequiresLevel <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionRequiresLevel,
            NormalizeEnabled = false,
            Value = item.Properties.RequiresLevel,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, TradeFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        query.Filters.GetOrCreateRequirementsFilters().Filters.Level = new StatFilterValue(intFilter);
    }
}
