using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RareMonstersProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionRareMonsters.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionRareMonsters.ToRegexIsAugmented();

    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Areas,
    ];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.RareMonsters = GetInt(Pattern, propertyBlock);
        if (item.Properties.RareMonsters == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.RareMonsters));
    }

    public override Task<PropertyFilter?> GetFilter(Item item)
    {
        if (item.Properties.RareMonsters <= 0) return Task.FromResult<PropertyFilter?>(null);

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionRareMonsters,
            NormalizeEnabled = true,
            Value = item.Properties.RareMonsters,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.RareMonsters)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        query.Filters.GetOrCreateMapFilters().Filters.RareMonsters = new StatFilterValue(intFilter);
    }
}
