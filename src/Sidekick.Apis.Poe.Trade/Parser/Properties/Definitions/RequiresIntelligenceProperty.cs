using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RequiresIntelligenceProperty(
    GameType game,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionRequiresInt.ToRegexIntCapture();

    private Regex RequiresPattern { get; } = new($@"^{gameLanguageProvider.Language.DescriptionRequires}.*?(\d+)\s*{gameLanguageProvider.Language.DescriptionRequiresInt}");

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Equipment,
        ..ItemClassConstants.Weapons,
        ItemClass.Graft,
    ];

    public override string Label => gameLanguageProvider.Language.DescriptionRequiresInt;

    public override void Parse(Item item)
    {
        foreach (var block in item.Text.Blocks)
        {
            item.Properties.RequiresIntelligence = GetInt(Pattern, block);
            if (item.Properties.RequiresIntelligence == 0) item.Properties.RequiresIntelligence = GetInt(RequiresPattern, block);
            if (item.Properties.RequiresIntelligence == 0) continue;

            block.Parsed = true;
            return;
        }
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
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateRequirementsFilters().Filters.Intelligence = new StatFilterValue(this);
    }
}
