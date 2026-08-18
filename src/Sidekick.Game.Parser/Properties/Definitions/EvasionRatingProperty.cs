using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using Sidekick.Game.Providers;
using ItemProperties = Sidekick.Game.Parser.Items.ItemProperties;

namespace Sidekick.Game.Parser.Properties.Definitions;

public class EvasionRatingProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameTextProvider.Texts.ItemPropertyEvasionRating.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = gameTextProvider.Texts.ItemPropertyEvasionRating.ToRegexIsAugmented();

    public override string Label => gameTextProvider.Texts.ItemPropertyEvasionRating;

    public override void Parse(Item item)
    {
        if (item.Properties.Rarity != Rarity.Normal &&
            item.Properties.Rarity != Rarity.Magic &&
            item.Properties.Rarity != Rarity.Rare &&
            item.Properties.Rarity != Rarity.Unique) return;

        item.Properties.EvasionRating = GetInt(Pattern, item.Text);
        if (item.Properties.EvasionRating == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.EvasionRating));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.EvasionRating <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new EvasionRatingFilter(game)
        {
            Text = Label,
            Value = item.Properties.EvasionRatingWithQuality,
            OriginalValue = item.Properties.EvasionRating,
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.EvasionRating)),
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
