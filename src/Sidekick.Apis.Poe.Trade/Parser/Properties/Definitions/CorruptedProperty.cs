using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class CorruptedProperty(
    GameType game,
    ISettingsService settingsService,
    IGameLanguageProvider gameLanguageProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameLanguageProvider.Language.DescriptionCorrupted.ToRegexLine();

    public override List<ItemClass> ValidItemClasses { get; } =
    [
        ..ItemClassConstants.Gems,
        ..ItemClassConstants.WithStats,
    ];

    public override void Parse(Item item)
    {
        item.Properties.Corrupted = GetBool(Pattern, item.Text);
    }

    public override async Task<TradeFilter?> GetFilter(Item item)
    {
        var autoSelectKey = $"Trade_Filter_{nameof(CorruptedProperty)}_{game.GetValueAttribute()}";
        var filter = new CorruptedFilter
        {
            Text = gameLanguageProvider.Language.DescriptionCorrupted,
            AutoSelectSettingKey = autoSelectKey,
            AutoSelect = await settingsService.GetObject<AutoSelectPreferences>(autoSelectKey, () => null),
        };
        return filter;
    }
}

public class CorruptedFilter : TriStatePropertyFilter
{
    public CorruptedFilter()
    {
        DefaultAutoSelect = new AutoSelectPreferences()
        {
            Mode = AutoSelectMode.Conditionally,
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
                            ComparisonType = AutoSelectComparisonType.Equals,
                            Value = true,
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
                            ComparisonType = AutoSelectComparisonType.Equals,
                            Value = false,
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
