using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.Parser.Items;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class CorruptedProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage) : PropertyDefinition
{
    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionCorrupted.ToRegexLine();

    public override string Label => currentGameLanguage.Language.DescriptionCorrupted;

    public override void Parse(Item item)
    {
        if (item.Properties.Rarity != Rarity.Normal &&
            item.Properties.Rarity != Rarity.Magic &&
            item.Properties.Rarity != Rarity.Rare &&
            item.Properties.Rarity != Rarity.Unique &&
            item.Properties.Rarity != Rarity.Gem) return;

        item.Properties.Corrupted = GetBool(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.Rarity != Rarity.Normal &&
            item.Properties.Rarity != Rarity.Magic &&
            item.Properties.Rarity != Rarity.Rare &&
            item.Properties.Rarity != Rarity.Unique &&
            item.Properties.Rarity != Rarity.Gem) return Task.FromResult<TradeFilter?>(null);

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
