using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Modifiers;

public class ModifierFilter
{
    public ModifierFilter(Modifier modifier)
    {
        Modifier = modifier;
        Checked = modifier.ApiInformation.FirstOrDefault()?.Category == ModifierCategory.Fractured;

        var categories = modifier.ApiInformation.Select(x => x.Category).Distinct().ToList();
        if (categories.Any(x => x is ModifierCategory.Fractured or ModifierCategory.Desecrated or ModifierCategory.Crafted))
        {
            UsePrimaryCategory = true;
            PrimaryCategory = categories.FirstOrDefault(x => x is ModifierCategory.Fractured or ModifierCategory.Desecrated or ModifierCategory.Crafted);
            SecondaryCategory = categories.FirstOrDefault(x => x == ModifierCategory.Explicit);
        }
        else
        {
            UsePrimaryCategory = false;
            PrimaryCategory = Modifier.ApiInformation.FirstOrDefault()?.Category ?? ModifierCategory.Undefined;
            SecondaryCategory = ModifierCategory.Undefined;
        }
    }

    public bool Checked { get; set; }

    public Modifier Modifier { get; }

    public bool UsePrimaryCategory { get; set; }

    public ModifierCategory PrimaryCategory { get; private init; }

    public ModifierCategory SecondaryCategory { get; private init; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked || Modifier.ApiInformation.Count == 0)
        {
            return;
        }

        if (Modifier.ApiInformation.Count == 1)
        {
            query.GetOrCreateStatGroup(StatType.And).Filters.Add(new StatFilters()
            {
                Id = Modifier.ApiInformation.First().ApiId,
                Value = new StatFilterValue(this),
            });
        }
        else
        {
            var modifiers = Modifier.ApiInformation.ToList();
            if (UsePrimaryCategory)
            {
                modifiers = modifiers.Where(x => x.Category == PrimaryCategory).ToList();
            }
            else if (SecondaryCategory != ModifierCategory.Undefined)
            {
                modifiers = modifiers.Where(x => x.Category == SecondaryCategory).ToList();
            }

            var countGroup = query.GetOrCreateStatGroup(StatType.Count);
            foreach (var modifier in modifiers)
            {
                countGroup.Filters.Add(new StatFilters()
                {
                    Id = modifier.ApiId,
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

            if (modifiers.Count != 0)
            {
                countGroup.Value.Min += 1;
            }
        }
    }
}
