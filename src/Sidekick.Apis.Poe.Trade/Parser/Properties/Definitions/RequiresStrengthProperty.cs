using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RequiresStrengthProperty(
    GameType game,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionRequiresStr.ToRegexIntCapture();

    private Regex RequiresPattern { get; } = new($@"^{gameLanguageProvider.Language.DescriptionRequires}.*?(\d+)\s*{gameLanguageProvider.Language.DescriptionRequiresStr}");

    public override string Label => gameLanguageProvider.Language.DescriptionRequiresStr;

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
