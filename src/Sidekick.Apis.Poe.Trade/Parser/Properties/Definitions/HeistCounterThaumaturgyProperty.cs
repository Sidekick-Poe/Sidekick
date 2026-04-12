using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.ItemClasses;
using Sidekick.Data.Items;
using Sidekick.Data.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class HeistCounterThaumaturgyProperty(
    GameType game,
    ICurrentGameLanguage currentGameLanguage,
    IServiceProvider serviceProvider) : PropertyDefinition
{
    private readonly ITradeFilterProvider tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();

    public override string Label => tradeFilterProvider.HeistCategory?.Filters.FirstOrDefault(x => x.Id == "heist_counter_thaumaturgy")?.Text ?? "Counter-Thaum. Level";

    private Regex Pattern { get; } = currentGameLanguage.Language.DescriptionHeistCounterThaumaturgy.ToRegexHeistLevelCapture();

    public override void Parse(Item item)
    {
        if (item.Text.Blocks.Count < 2) return;
        if (game != GameType.PathOfExile1) return;

        item.Properties.HeistCounterThaumaturgyLevel = GetInt(Pattern, item.Text);
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.HeistCounterThaumaturgyLevel <= 0) return Task.FromResult<TradeFilter?>(null);
        var filter = new HeistCounterThaumaturgyFilter
        {
            Text = Label,
            Value = item.Properties.HeistCounterThaumaturgyLevel,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(HeistCounterThaumaturgyProperty)}_{game.GetValueAttribute()}",
            NormalizeEnabled = false,
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class HeistCounterThaumaturgyFilter : IntPropertyFilter
{
    public HeistCounterThaumaturgyFilter() {
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
        query.Filters.GetOrCreateHeistFilters().Filters.CounterThaumaturgy = new StatFilterValue(this);
    }
}
