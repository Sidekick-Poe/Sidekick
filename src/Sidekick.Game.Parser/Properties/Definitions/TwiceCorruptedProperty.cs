using System.Text.RegularExpressions;
using Sidekick.Game.Enums;
using Sidekick.Game.Parser.Filters.AutoSelect;
using Sidekick.Game.Parser.Filters.Types;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Parser.Trade.Requests;
using Sidekick.Game.Parser.Trade.Requests.Filters;
using Sidekick.Game.Providers;
namespace Sidekick.Game.Parser.Properties.Definitions;

/// <remarks>
/// Must be parsed after <see cref="CorruptedProperty"/>. The game prints either the corrupted line or the twice corrupted line, never both, and a twice corrupted item is also corrupted.
/// </remarks>
public class TwiceCorruptedProperty(
    GameType game,
    GameTextProvider gameTextProvider) : PropertyDefinition
{
    private Regex? Pattern { get; } = string.IsNullOrEmpty(gameTextProvider.Texts.ItemPropertyTwiceCorrupted) ? null : gameTextProvider.Texts.ItemPropertyTwiceCorrupted.ToRegexLine();

    public override string Label => gameTextProvider.Texts.ItemPropertyTwiceCorrupted;

    public override void Parse(Item item)
    {
        if (Pattern == null) return;

        if (item.Properties.Rarity != Rarity.Normal &&
            item.Properties.Rarity != Rarity.Magic &&
            item.Properties.Rarity != Rarity.Rare &&
            item.Properties.Rarity != Rarity.Unique &&
            item.Properties.Rarity != Rarity.Gem) return;

        item.Properties.TwiceCorrupted = GetBool(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.Rarity != Rarity.Normal &&
            item.Properties.Rarity != Rarity.Magic &&
            item.Properties.Rarity != Rarity.Rare &&
            item.Properties.Rarity != Rarity.Unique &&
            item.Properties.Rarity != Rarity.Gem) return Task.FromResult<TradeFilter?>(null);

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
