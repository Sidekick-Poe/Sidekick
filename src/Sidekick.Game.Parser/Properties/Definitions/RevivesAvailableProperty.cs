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

public class RevivesAvailableProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameTextProvider.Texts.ItemPropertyRevivesAvailable.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = gameTextProvider.Texts.ItemPropertyRevivesAvailable.ToRegexIsAugmented();

    public override string Label => gameTextProvider.Texts.ItemPropertyRevivesAvailable;

    public override void Parse(Item item)
    {
        if (game != GameType.Poe2) return;

        item.Properties.RevivesAvailable = GetInt(Pattern, item.Text);
        if (item.Properties.RevivesAvailable == 0) return;

        if (GetBool(IsAugmentedPattern, item.Text)) item.Properties.AugmentedProperties.Add(nameof(ItemProperties.RevivesAvailable));
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.RevivesAvailable <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new RevivesAvailableFilter
        {
            Text = Label,
            Value = item.Properties.RevivesAvailable,
            Augmented = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.RevivesAvailable)),
            AutoSelectSettingKey = $"Trade_Filter_{nameof(RevivesAvailableProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class RevivesAvailableFilter : IntPropertyFilter
{
    public RevivesAvailableFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.RevivesAvailable = new StatFilterValue(this);
    }
}
