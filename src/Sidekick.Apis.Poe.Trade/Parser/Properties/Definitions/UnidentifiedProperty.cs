using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class UnidentifiedProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionUnidentified.ToRegexLine();

    public override string Label => currentGameLanguage.Language.DescriptionUnidentified;

    public override void Parse(Item item)
    {
        item.Properties.Unidentified = GetBool(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.ItemClass.IsEquipment() &&
            !item.ItemClass.IsWeapon() &&
            !item.ItemClass.IsAccessory() &&
            !item.ItemClass.IsFlask() &&
            !item.ItemClass.IsArea() &&
            !item.ItemClass.IsJewel()) return Task.FromResult<TradeFilter?>(null);

        var filter = new UnidentifiedFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(UnidentifiedProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class UnidentifiedFilter : TriStatePropertyFilter
{
    public UnidentifiedFilter()
    {
        DefaultAutoSelect = new AutoSelectPreferences()
        {
            Mode = AutoSelectMode.Default,
            Rules =
            [
                new()
                {
                    Checked = true,
                    Conditions =
                    [
                        new()
                        {
                            Type = AutoSelectConditionType.Unidentified,
                            Comparison = AutoSelectComparisonType.True,
                        },
                    ],
                },
                new()
                {
                    Checked = null,
                },
            ],
        };
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (Checked is null)
        {
            return;
        }

        query.Filters.GetOrCreateMiscFilters().Filters.Identified = new SearchFilterOption(Checked is true ? "false" : "true");
    }
}
