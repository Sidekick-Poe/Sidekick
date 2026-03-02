using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class RequiresDexterityProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionRequiresDex.ToRegexIntCapture();

    private Regex RequiresPattern { get; } = new($@"^{currentGameLanguage.Language.DescriptionRequires}.*?(\d+)\s*{currentGameLanguage.Language.DescriptionRequiresDex}");

    public override string Label => currentGameLanguage.Language.DescriptionRequiresDex;

    public override void Parse(Item item)
    {
        if (!ItemClassConstants.Equipment.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Weapons.Contains(item.Properties.ItemClass) &&
            item.Properties.ItemClass != ItemClass.Graft) return;

        foreach (var block in item.Text.Blocks)
        {
            item.Properties.RequiresDexterity = GetInt(Pattern, block);
            if (item.Properties.RequiresDexterity == 0) item.Properties.RequiresDexterity = GetInt(RequiresPattern, block);
            if (item.Properties.RequiresDexterity == 0) continue;

            block.Parsed = true;
            return;
        }
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.RequiresDexterity <= 0) return Task.FromResult<TradeFilter?>(null);

        var filter = new RequiresDexterityFilter
        {
            Text = Label,
            Value = item.Properties.RequiresDexterity,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(RequiresDexterityProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class RequiresDexterityFilter : IntPropertyFilter
{
    public RequiresDexterityFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false, normalizeBy: 0);
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

        query.Filters.GetOrCreateRequirementsFilters().Filters.Dexterity = new StatFilterValue(this);
    }
}
