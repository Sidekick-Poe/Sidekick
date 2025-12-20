using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;

public class StatFilter
{
    public StatFilter(Stat stat)
    {
        Stat = stat;
        Checked = stat.ApiInformation.FirstOrDefault()?.Category == StatCategory.Fractured;

        var categories = stat.ApiInformation.Select(x => x.Category).Distinct().ToList();
        if (categories.Any(x => x is StatCategory.Fractured or StatCategory.Desecrated or StatCategory.Crafted))
        {
            UsePrimaryCategory = true;
            PrimaryCategory = categories.FirstOrDefault(x => x is StatCategory.Fractured or StatCategory.Desecrated or StatCategory.Crafted);
            SecondaryCategory = categories.FirstOrDefault(x => x == StatCategory.Explicit);
        }
        else
        {
            UsePrimaryCategory = false;
            PrimaryCategory = Stat.ApiInformation.FirstOrDefault()?.Category ?? StatCategory.Undefined;
            SecondaryCategory = StatCategory.Undefined;
        }
    }

    public bool Checked { get; set; }

    public Stat Stat { get; }

    public bool UsePrimaryCategory { get; set; }

    public StatCategory PrimaryCategory { get; private init; }

    public StatCategory SecondaryCategory { get; private init; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked || Stat.ApiInformation.Count == 0)
        {
            return;
        }

        if (Stat.ApiInformation.Count == 1)
        {
            query.GetOrCreateStatGroup(StatType.And).Filters.Add(new StatFilters()
            {
                Id = Stat.ApiInformation.First().Id,
                Value = new StatFilterValue(this),
            });
        }
        else
        {
            var stats = Stat.ApiInformation.ToList();
            if (UsePrimaryCategory)
            {
                stats = stats.Where(x => x.Category == PrimaryCategory).ToList();
            }
            else if (SecondaryCategory != StatCategory.Undefined)
            {
                stats = stats.Where(x => x.Category == SecondaryCategory).ToList();
            }

            var countGroup = query.GetOrCreateStatGroup(StatType.Count);
            foreach (var stat in stats)
            {
                countGroup.Filters.Add(new StatFilters()
                {
                    Id = stat.Id,
                    Value = new StatFilterValue(this),
                });
            }

            if (countGroup.Value == null)
            {
                countGroup.Value = new StatFilterValue()
                {
                    Min = 0,
                };
            }

            if (stats.Count != 0)
            {
                countGroup.Value.Min += 1;
            }
        }
    }
}
