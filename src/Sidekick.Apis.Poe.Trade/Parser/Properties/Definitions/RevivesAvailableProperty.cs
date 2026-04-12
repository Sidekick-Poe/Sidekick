using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Results;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;
using ItemProperties = Sidekick.Data.Items.ItemProperties;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RevivesAvailableProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionRevivesAvailable.ToRegexIntProperty();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionRevivesAvailable.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionRevivesAvailable;

    public override void Parse(Item item)
    {
        if (game != GameType.PathOfExile2) return;

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
            Type = item.Properties.AugmentedProperties.Contains(nameof(ItemProperties.RevivesAvailable)) ? LineContentType.Augmented : LineContentType.Simple,
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
