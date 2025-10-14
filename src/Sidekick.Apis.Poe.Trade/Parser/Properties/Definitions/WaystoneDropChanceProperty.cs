using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class WaystoneDropChanceProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionWaystoneDropChance.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionWaystoneDropChance.ToRegexIsAugmented();

    public override List<Category> ValidCategories { get; } = [Category.Map, Category.Contract, Category.Logbook];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.WaystoneDropChance = GetInt(Pattern, propertyBlock);
        if (item.Properties.WaystoneDropChance == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.WaystoneDropChance));
    }

    public override Task<PropertyFilter?> GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.WaystoneDropChance <= 0) return Task.FromResult<PropertyFilter?>(null);

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionWaystoneDropChance,
            NormalizeEnabled = true,
            NormalizeValue = normalizeValue,
            Value = item.Properties.WaystoneDropChance,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.WaystoneDropChance)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        filter.ChangeFilterType(filterType);
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        query.Filters.GetOrCreateMapFilters().Filters.WaystoneDropChance = new StatFilterValue(intFilter);
    }
}
