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
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionLevel.ToRegexIntCapture();

    private Regex RequiresPattern { get; } = new($@"^{currentGameLanguage.Language.DescriptionRequires}.*{currentGameLanguage.Language.DescriptionLevel}\s*(\d+)");

    public override string Label => currentGameLanguage.Language.DescriptionRequiresLevel;

    public override void Parse(Item item)
    {
        if (!ItemClassConstants.Equipment.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Weapons.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Accessories.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Flasks.Contains(item.Properties.ItemClass) &&
            item.Properties.ItemClass != ItemClass.Graft) return;

        foreach (var block in item.Text.Blocks)
        {
            item.Properties.RequiresLevel = GetInt(Pattern, block);
            if (item.Properties.RequiresLevel == 0) item.Properties.RequiresLevel = GetInt(RequiresPattern, block);
            if (item.Properties.RequiresLevel == 0) continue;

            block.Parsed = true;
            return;
        }
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.RequiresLevel <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new RequiresLevelFilter
        {
            Text = Label,
            Value = item.Properties.RequiresLevel,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(RequiresLevelProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
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
