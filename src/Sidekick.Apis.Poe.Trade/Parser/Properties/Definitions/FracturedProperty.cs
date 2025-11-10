using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class FracturedProperty(IServiceProvider serviceProvider) : PropertyDefinition
{
    private IFilterProvider FilterProvicer => serviceProvider.GetRequiredService<IFilterProvider>();

    public override List<Category> ValidCategories { get; } = [Category.Armour, Category.Weapon, Category.Accessory];

    public override void Parse(Item item)
    {
    }

    public override Task<PropertyFilter?> GetFilter(Item item)
    {
        if (FilterProvicer.Fractured == null) return Task.FromResult<PropertyFilter?>(null);

        var filter = new TriStatePropertyFilter(this)
        {
            Text = FilterProvicer.Fractured.Text ?? "Fractured",
            Checked = null,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (filter is not TriStatePropertyFilter triStateFilter || triStateFilter.Checked == null)
        {
            return;
        }

        query.Filters.GetOrCreateMiscFilters().Filters.Fractured = new SearchFilterOption(triStateFilter);
    }
}
