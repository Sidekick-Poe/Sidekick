using System.Text.RegularExpressions;
using Sidekick.Game.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using Sidekick.Game.Providers;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class FoulbornProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex Pattern { get; } = gameTextProvider.Texts.ItemFoulborn.ToRegexStringProperty();

    public override string Label => gameTextProvider.Texts.ItemFoulborn.CleanWildcard();

    public override void Parse(Item item)
    {
        item.Properties.Foulborn = GetBool(Pattern, item.Text.Blocks[0]);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (game == GameType.Poe2 || item.Properties.Rarity != Rarity.Unique) return Task.FromResult<TradeFilter?>(null);

        var filter = new FoulbornFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(FoulbornProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class FoulbornFilter : TriStatePropertyFilter
{
    public FoulbornFilter()
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
                            Type = AutoSelectConditionType.Foulborn,
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
                            Type = AutoSelectConditionType.Foulborn,
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

        query.Filters.GetOrCreateMiscFilters().Filters.Foulborn = new SearchFilterOption(this);
    }
}
