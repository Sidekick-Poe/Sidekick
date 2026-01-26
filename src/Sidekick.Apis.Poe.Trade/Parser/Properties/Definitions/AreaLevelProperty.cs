using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class AreaLevelProperty(
    GameType game,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionAreaLevel.ToRegexIntCapture();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Areas,
    ];

    public override string Label => gameLanguageProvider.Language.DescriptionAreaLevel;

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.AreaLevel = GetInt(Pattern, propertyBlock);
        if (item.Properties.AreaLevel > 0) propertyBlock.Parsed = true;
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.AreaLevel <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new AreaLevelFilter
        {
            Text = Label,
            Value = item.Properties.AreaLevel,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(AreaLevelProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class AreaLevelFilter : IntPropertyFilter
{
    public AreaLevelFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.AreaLevel = new StatFilterValue(this);
    }
}
