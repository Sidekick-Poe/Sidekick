using System.Text.RegularExpressions;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class AreaLevelProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionAreaLevel.ToRegexIntProperty();

    public override string Label => currentGameLanguage.Language.DescriptionAreaLevel;

    public override void Parse(Item item)
    {
        item.Properties.AreaLevel = GetInt(Pattern, item.Text);
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
        DefaultAutoSelect = AutoSelectPreferences.Create(true, normalizeBy: 0);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateMapFilters().Filters.AreaLevel = new StatFilterValue(this);
    }
}
