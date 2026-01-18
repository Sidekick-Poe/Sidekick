using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RequiresLevelProperty(
    GameType game,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionLevel.ToRegexIntCapture();

    private Regex RequiresPattern { get; } = new($@"^{gameLanguageProvider.Language.DescriptionRequires}.*{gameLanguageProvider.Language.DescriptionLevel}\s*(\d+)");

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Equipment,
        ..ItemClassConstants.Weapons,
        ..ItemClassConstants.Accessories,
        ..ItemClassConstants.Flasks,
        ItemClass.Graft,
    ];

    public override string Label => gameLanguageProvider.Language.DescriptionRequiresLevel;

    public override void Parse(Item item)
    {
        foreach (var block in item.Text.Blocks)
        {
            item.Properties.RequiresLevel = GetInt(Pattern, block);
            if (item.Properties.RequiresLevel == 0) item.Properties.RequiresLevel = GetInt(RequiresPattern, block);
            if (item.Properties.RequiresLevel == 0) continue;

            block.Parsed = true;
            return;
        }
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.RequiresLevel <= 0) return null;

        var filter = new RequiresLevelFilter
        {
            Text = Label,
            Value = item.Properties.RequiresLevel,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(RequiresLevelProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return filter;
    }
}

public class RequiresLevelFilter : IntPropertyFilter
{
    public RequiresLevelFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateRequirementsFilters().Filters.Level = new StatFilterValue(this);
    }
}
