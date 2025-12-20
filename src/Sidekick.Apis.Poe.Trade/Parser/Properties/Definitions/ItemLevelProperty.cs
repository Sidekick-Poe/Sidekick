using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Definitions;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ItemLevelProperty(IGameLanguageProvider gameLanguageProvider, GameType game) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionItemLevel.ToRegexIntCapture();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Equipment,
        ..ItemClassConstants.Weapons,
        ..ItemClassConstants.Accessories,

        ..ItemClassConstants.Flasks,
        ..ItemClassConstants.Jewels,
        ..ItemClassConstants.Areas,

        ItemClass.SanctumRelic,
        ItemClass.SanctumResearch,

        ItemClass.Wombgift,
        ItemClass.Graft,
    ];

    public override void Parse(Item item)
    {
        item.Properties.ItemLevel = GetInt(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.ItemLevel <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new IntPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionItemLevel,
            NormalizeEnabled = false,
            Value = item.Properties.ItemLevel,
            Checked = game == GameType.PathOfExile1 && item.Properties.ItemLevel >= 80 && item.Properties.MapTier == 0 && item.Properties.Rarity != Rarity.Unique,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }

    public override void PrepareTradeRequest(Query query, Item item, TradeFilter filter)
    {
        if (!filter.Checked || filter is not IntPropertyFilter intFilter) return;

        switch (game)
        {
            case GameType.PathOfExile1: query.Filters.GetOrCreateMiscFilters().Filters.ItemLevel = new StatFilterValue(intFilter); break;
            case GameType.PathOfExile2: query.Filters.GetOrCreateTypeFilters().Filters.ItemLevel = new StatFilterValue(intFilter); break;
        }
    }
}
