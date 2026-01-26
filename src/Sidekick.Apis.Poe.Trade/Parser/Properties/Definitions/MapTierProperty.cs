using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class MapTierProperty(
    GameType game,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionMapTier.ToRegexIntCapture();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ItemClass.Map,
    ];

    public override string Label => gameLanguageProvider.Language.DescriptionMapTier;

    public override void Parse(Item item)
    {
        var propertyBlock = item.Text.Blocks[1];
        item.Properties.MapTier = GetInt(Pattern, propertyBlock);
        if (item.Properties.MapTier > 0) propertyBlock.Parsed = true;
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.MapTier <= 0) return null;

        var filter = new MapTierFilter
        {
            Text = Label,
            Value = item.Properties.MapTier,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(MapTierProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = true,
        };
        return filter;
    }
}

public class MapTierFilter : IntPropertyFilter
{
    public MapTierFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.MapTier = new StatFilterValue(this);
    }
}
