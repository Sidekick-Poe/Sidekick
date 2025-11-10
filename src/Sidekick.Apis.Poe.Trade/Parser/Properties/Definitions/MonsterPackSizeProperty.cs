using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class MonsterPackSizeProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionMonsterPackSize.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionMonsterPackSize.ToRegexIsAugmented();

    public override List<Category> ValidCategories { get; } = [Category.Map, Category.Contract, Category.Logbook];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.MonsterPackSize = GetInt(Pattern, propertyBlock);
        if (item.Properties.MonsterPackSize == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.MonsterPackSize));
    }

    public override Task<PropertyFilter?> GetFilter(Item item)
    {
        if (item.Properties.MonsterPackSize <= 0) return Task.FromResult<PropertyFilter?>(null);

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionMonsterPackSize,
            NormalizeEnabled = true,
            Value = item.Properties.MonsterPackSize,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.MonsterPackSize)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        return Task.FromResult<PropertyFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        query.Filters.GetOrCreateMapFilters().Filters.MonsterPackSize = new StatFilterValue(intFilter);
    }
}
