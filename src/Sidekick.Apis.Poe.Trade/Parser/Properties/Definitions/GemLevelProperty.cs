using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class GemLevelProperty(
    GameType game,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionLevel.ToRegexIntCapture();

    public override List<ItemClass> ValidItemClasses { get; } = [..ItemClassConstants.Gems];

    public override string Label => gameLanguageProvider.Language.DescriptionLevel;

    public override void Parse(Item item)
    {
        if (item.Properties.ItemClass is ItemClass.UncutSkillGem or ItemClass.UncutSupportGem or ItemClass.UncutSpiritGem)
        {
            return;
        }

        var propertyBlock = item.Text.Blocks[1];
        item.Properties.GemLevel = GetInt(Pattern, propertyBlock);

        if (item.Properties.GemLevel > 0) propertyBlock.Parsed = true;
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.GemLevel <= 0) return null;

        var filter = new GemLevelFilter
        {
            Text = Label,
            Value = item.Properties.GemLevel,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(GemLevelProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return filter;
    }
}

public class GemLevelFilter : IntPropertyFilter
{
    public GemLevelFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(true);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.GemLevel = new StatFilterValue(this);
    }
}
