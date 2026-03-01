using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Enums;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class EvasionRatingProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionEvasion.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionEvasion.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionEvasion;

    public override void Parse(Item item)
    {
        if (!ItemClassConstants.Equipment.Contains(item.Properties.ItemClass)) return;

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
            Text = Label,
            Value = item.Properties.EvasionRatingWithQuality,
            OriginalValue = item.Properties.EvasionRating,
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.EvasionRating)) ? LineContentType.Augmented : LineContentType.Simple,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(EvasionRatingProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class EvasionRatingFilter : IntPropertyFilter
{
    public EvasionRatingFilter(GameType game)
    {
        Game = game;
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    private GameType Game { get; }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        switch (Game)
        {
            case GameType.PathOfExile1: query.Filters.GetOrCreateArmourFilters().Filters.EvasionRating = new StatFilterValue(this); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateEquipmentFilters().Filters.EvasionRating = new StatFilterValue(this); break;
        }
    }
}
