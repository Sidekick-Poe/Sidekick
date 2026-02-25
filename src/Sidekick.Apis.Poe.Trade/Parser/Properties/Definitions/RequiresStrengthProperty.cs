using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data.Items.Models;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RequiresStrengthProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionRequiresStr.ToRegexIntCapture();

    private Regex RequiresPattern { get; } = new($@"^{currentGameLanguage.Language.DescriptionRequires}.*?(\d+)\s*{currentGameLanguage.Language.DescriptionRequiresStr}");

    public override string Label => currentGameLanguage.Language.DescriptionRequiresStr;

    public override void Parse(Item item)
    {
        if (!ItemClassConstants.Equipment.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Weapons.Contains(item.Properties.ItemClass) &&
            item.Properties.ItemClass != ItemClass.Graft) return;

        foreach (var block in item.Text.Blocks)
        {
            item.Properties.RequiresStrength = GetInt(Pattern, block);
            if (item.Properties.RequiresStrength == 0) item.Properties.RequiresStrength = GetInt(RequiresPattern, block);
            if (item.Properties.RequiresStrength == 0) continue;

            block.Parsed = true;
            return;
        }
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.RequiresStrength <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new RequiresStrengthFilter
        {
            Text = Label,
            Value = item.Properties.RequiresStrength,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(RequiresStrengthProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class RequiresStrengthFilter : IntPropertyFilter
{
    public RequiresStrengthFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateRequirementsFilters().Filters.Strength = new StatFilterValue(this);
    }
}
