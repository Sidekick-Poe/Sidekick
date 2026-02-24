using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class UnidentifiedProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionUnidentified.ToRegexLine();

    public override string Label => currentGameLanguage.Language.DescriptionUnidentified;

    public override void Parse(Item item)
    {
        if (!ItemClassConstants.Equipment.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Weapons.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Accessories.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Flasks.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Jewels.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Areas.Contains(item.Properties.ItemClass)) return;

        item.Properties.Unidentified = GetBool(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!ItemClassConstants.Equipment.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Weapons.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Accessories.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Flasks.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Jewels.Contains(item.Properties.ItemClass) &&
            !ItemClassConstants.Areas.Contains(item.Properties.ItemClass)) return Task.FromResult<TradeFilter?>(null);

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
