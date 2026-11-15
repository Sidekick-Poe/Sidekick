using Sidekick.Game.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using Sidekick.Game.Providers;
namespace Sidekick.Game.Parser.Properties.Definitions;

/// <remarks>
/// Parsing happens in <see cref="CorruptedProperty"/>, since a twice corrupted item is also corrupted.
/// </remarks>
public class TwiceCorruptedProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    public override string Label => gameTextProvider.Texts.ItemPropertyTwiceCorrupted;

    public override void Parse(Item item) {}

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (!item.Properties.Corrupted || string.IsNullOrEmpty(Label)) return Task.FromResult<TradeFilter?>(null);

        var filter = new TwiceCorruptedFilter
        {
            Text = Label,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(TwiceCorruptedProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class TwiceCorruptedFilter : TriStatePropertyFilter
{
    public TwiceCorruptedFilter()
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
                            Type = AutoSelectConditionType.TwiceCorrupted,
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
        if (Checked == null)
        {
            return;
        }

        query.Filters.GetOrCreateMiscFilters().Filters.TwiceCorrupted = new SearchFilterOption(this);
    }
}
