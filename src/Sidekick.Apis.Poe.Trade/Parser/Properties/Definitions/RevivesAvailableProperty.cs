using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Items.Results;
using Sidekick.Common.Enums;
using Sidekick.Data.Items.Models;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RevivesAvailableProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionRevivesAvailable.ToRegexIntCapture();

    private Regex IsAugmentedPattern { get; } = currentGameLanguage.Language.DescriptionRevivesAvailable.ToRegexIsAugmented();

    public override string Label => currentGameLanguage.Language.DescriptionRevivesAvailable;

    public override void Parse(Item item)
    {
        if (!ItemClassConstants.Areas.Contains(item.Properties.ItemClass)) return;

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
