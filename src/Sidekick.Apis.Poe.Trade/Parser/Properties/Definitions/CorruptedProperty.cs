using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class CorruptedProperty(
    GameType game,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionCorrupted.ToRegexLine();

    public override string Label => gameLanguageProvider.Language.DescriptionCorrupted;

    public override void Parse(Item item)
    {
        item.Properties.Corrupted = GetBool(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        var filter = new CorruptedFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(CorruptedProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class CorruptedFilter : TriStatePropertyFilter
{
    public CorruptedFilter()
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
                            Type = AutoSelectConditionType.Corrupted,
                            Comparison = AutoSelectComparisonType.True,
                        },
                    ],
                },
                new()
                {
                    Checked = false,
                    Conditions =
                    [
                        new()
                        {
                            Type = AutoSelectConditionType.Corrupted,
                            Comparison = AutoSelectComparisonType.False,
                        },
                    ],
                },
            ],
        };
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (Checked == null)
        {
            return;
        }

        query.Filters.GetOrCreateMiscFilters().Filters.Corrupted = new SearchFilterOption(this);
    }
}
