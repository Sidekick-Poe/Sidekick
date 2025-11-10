using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RedeemerProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.InfluenceRedeemer.ToRegexLine();

    public override List<Category> ValidCategories { get; } = [Category.Armour, Category.Weapon, Category.Accessory, Category.Jewel];

    public override void Parse(Item item)
    {
        item.Properties.Influences.Redeemer = GetBool(Pattern, item.Text);
    }

    public override Task<PropertyFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Influences.Redeemer) return Task.FromResult<PropertyFilter?>(null);

        var filter = new PropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.InfluenceRedeemer,
            Checked = true,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.RedeemerItem = new SearchFilterOption(filter);
    }
}
