using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RequiresIntelligenceProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionRequiresInt.ToRegexIntProperty();

    private Regex RequiresPattern { get; } = new($@"^{currentGameLanguage.Language.DescriptionRequires}.*?(\d+)\s*{currentGameLanguage.Language.DescriptionRequiresInt}");

    public override string Label => currentGameLanguage.Language.DescriptionRequiresInt;

    public override void Parse(Item item)
    {
        var block = item.Text.Blocks.FirstOrDefault(x => x.Type == RawBlockType.Requirements);
        if (block == null) return;

        item.Properties.RequiresIntelligence = GetInt(Pattern, block);
        if (item.Properties.RequiresIntelligence == 0) item.Properties.RequiresIntelligence = GetInt(RequiresPattern, block);
        if (item.Properties.RequiresIntelligence == 0) return;

        block.Parsed = true;
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.RequiresIntelligence <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new RequiresIntelligenceFilter
        {
            Text = Label,
            Value = item.Properties.RequiresIntelligence,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(RequiresIntelligenceProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class RequiresIntelligenceFilter : IntPropertyFilter
{
    public RequiresIntelligenceFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false, normalizeBy: 0);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateRequirementsFilters().Filters.Intelligence = new StatFilterValue(this);
    }
}
