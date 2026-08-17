using System.Text.Json;
using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Parser;
using Sidekick.Apis.Poe.Trade.Parser.Properties;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings.Languages;
using Sidekick.Game.ItemClasses;
using Sidekick.Game.Parser.Items;
namespace Sidekick.Game.Parser.Properties.Definitions;

public class HeistTrapDisarmamentProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage,
    TradeFilterProvider tradeFilterProvider) : PropertyDefinition
{
    public override string Label => tradeFilterProvider.HeistCategory?.Filters.FirstOrDefault(x => x.Id == "heist_trap_disarmament")?.Text ?? "Trap Disarmament Level";

    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionHeistTrap.ToRegexHeistLevelCapture();

    public override void Parse(Item item)
    {
        if (game != GameType.PathOfExile1) return;

        item.Properties.HeistTrapDisarmamentLevel = GetInt(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.HeistTrapDisarmamentLevel <= 0) return Task.FromResult<TradeFilter?>(null);
        var filter = new HeistTrapDisarmamentFilter
        {
            Text = Label,
            Value = item.Properties.HeistTrapDisarmamentLevel,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(HeistTrapDisarmamentProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class HeistTrapDisarmamentFilter : IntPropertyFilter
{
    public HeistTrapDisarmamentFilter()
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
                            Type = AutoSelectConditionType.ItemClass,
                            Comparison = AutoSelectComparisonType.IsContainedIn,
                            Value = JsonSerializer.Serialize(new List<ItemClass>()
                            {
                                ItemClass.HeistContract,
                            }, AutoSelectPreferences.JsonSerializerOptions),
                        },
                    ],
                    NormalizeBy = 0,
                },
            ],
        };
    }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;
        query.Filters.GetOrCreateHeistFilters().Filters.TrapDisarmament = new StatFilterValue(this);
    }
}
