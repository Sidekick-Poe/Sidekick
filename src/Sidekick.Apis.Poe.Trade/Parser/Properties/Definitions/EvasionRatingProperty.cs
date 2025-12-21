using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class EvasionRatingProperty(IGameLanguageProvider gameLanguageProvider, GameType game) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionEvasion.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = gameLanguageProvider.Language.DescriptionEvasion.ToRegexIsAugmented();

    public override List<ItemClass> ValidItemClasses { get; } = [
        ..ItemClassConstants.Equipment,
    ];

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.EvasionRating = GetInt(Pattern, propertyBlock);
        if (item.Properties.EvasionRating == 0) return;

        propertyBlock.Parsed = true;
        if (GetBool(IsAugmentedPattern, propertyBlock)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.EvasionRating));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.EvasionRating <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new EvasionRatingFilter(game)
        {
            Text = gameLanguageProvider.Language.DescriptionEvasion,
            NormalizeEnabled = true,
            Value = item.Properties.EvasionRatingWithQuality,
            OriginalValue = item.Properties.EvasionRating,
            Checked = false,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.EvasionRating)) ? LineContentType.Augmented : LineContentType.Simple,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class EvasionRatingFilter(GameType game) : IntPropertyFilter
{
    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        switch (game)
        {
            case GameType.PathOfExile1: query.Filters.GetOrCreateArmourFilters().Filters.EvasionRating = new StatFilterValue(this); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.EvasionRating = new StatFilterValue(this); break;
        }
    }
}
