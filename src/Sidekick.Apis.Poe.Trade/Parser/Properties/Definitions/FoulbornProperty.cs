using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class FoulbornProperty(IServiceProvider serviceProvider, GameType game) : PropertyDefinition
{
    private IFilterProvider FilterProvicer => serviceProvider.GetRequiredService<IFilterProvider>();

    public override List<Category> ValidItemClasses { get; } = [Category.Armour, Category.Weapon, Category.Accessory, Category.Jewel, Category.Flask];

    public override void ParseAfterModifiers(Item item)
    {
        if (game == GameType.PathOfExile2) return;
        if (item.Properties.Rarity != Rarity.Unique) return;

        item.Properties.Foulborn = item.Modifiers.Any(x => x.ApiInformation.Any(y => y.Category == ModifierCategory.Mutated));
    }

    public override Task<PropertyFilter?> GetFilter(Item item)
    {
        if (game == GameType.PathOfExile2) return Task.FromResult<PropertyFilter?>(null);
        if (FilterProvicer.Foulborn == null) return Task.FromResult<PropertyFilter?>(null);
        if (item.Properties.Rarity != Rarity.Unique) return Task.FromResult<PropertyFilter?>(null);

        var filter = new TriStatePropertyFilter(this)
        {
            Text = FilterProvicer.Foulborn.Text ?? "Foulborn",
            Checked = item.Properties.Foulborn,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (filter is not TriStatePropertyFilter triStateFilter || triStateFilter.Checked == null)
        {
            return;
        }

        query.Filters.GetOrCreateMiscFilters().Filters.Foulborn = new SearchFilterOption(triStateFilter);
    }
}
