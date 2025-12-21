using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RevivesAvailableProperty(IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionRevivesAvailable.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionRevivesAvailable.ToRegexIsAugmented();

    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Areas,
    ];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.RevivesAvailable = GetInt(Pattern, propertyBlock);
        if (item.Properties.RevivesAvailable == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.RevivesAvailable));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.RevivesAvailable <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new RevivesAvailableFilter
        {
            Text = gameLanguageProvider.Language.DescriptionRevivesAvailable,
            NormalizeEnabled = false,
            Value = item.Properties.RevivesAvailable,
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.RevivesAvailable)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class RevivesAvailableFilter : IntPropertyFilter
{
    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.RevivesAvailable = new StatFilterValue(this);
    }
}
