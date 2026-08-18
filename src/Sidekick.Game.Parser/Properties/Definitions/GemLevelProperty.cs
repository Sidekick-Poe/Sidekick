using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class GemLevelProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionLevel.ToRegexIntProperty();

    public override string Label => currentGameLanguage.Language.DescriptionLevel;

    public override void Parse(Item item)
    {
        if (item.Properties.Rarity != Rarity.Gem) return;

        item.Properties.GemLevel = GetInt(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.GemLevel <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new GemLevelFilter
        {
            Text = Label,
            Value = item.Properties.GemLevel,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(GemLevelProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class GemLevelFilter : IntPropertyFilter
{
    public GemLevelFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(true, normalizeBy: 0);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMiscFilters().Filters.GemLevel = new StatFilterValue(this);
    }
}