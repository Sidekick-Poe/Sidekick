using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class QualityProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionQuality.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionQuality.ToRegexIsAugmented();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Equipment,
        ..ItemClassConstants.Weapons,
        ..ItemClassConstants.Accessories,
        ..ItemClassConstants.Flasks,
        ..ItemClassConstants.Gems,
        ..ItemClassConstants.Areas,
    ];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.Quality = GetInt(Pattern, propertyBlock);
        if (item.Properties.Quality == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.Quality));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.Quality <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new QualityFilter
        {
            Text = gameLanguageProvider.Language.DescriptionQuality,
            NormalizeEnabled = false,
            Value = item.Properties.Quality,
            ValuePrefix = "+",
            ValueSuffix = "%",
            Checked = item.Properties.Rarity == Rarity.Gem,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.Quality)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class QualityFilter : IntPropertyFilter
{
    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.Quality = new StatFilterValue(this);
    }
}
