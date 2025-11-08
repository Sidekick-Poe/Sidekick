using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class WarlordProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.InfluenceWarlord.ToRegexLine();

    public override List<Category> ValidItemClasses { get; } = [Category.Armour, Category.Weapon, Category.Accessory, Category.Jewel];

    public override void Parse(Item item)
    {
        item.Properties.Influences.Warlord = GetBool(Pattern, item.Text);
    }

    public override Task<PropertyFilter?> GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (!item.Properties.Influences.Warlord) return Task.FromResult<PropertyFilter?>(null);

        var filter = new PropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.InfluenceWarlord,
            Checked = true,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.WarlordItem = new SearchFilterOption(filter);
    }
}
